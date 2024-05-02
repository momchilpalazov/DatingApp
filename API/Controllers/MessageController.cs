using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class MessagesController:BaseController
{
    
    

    public IMapper _mapper ;

    public IUnitOfWorkRepository _unitOfWorkRepository;

    public MessagesController( IMapper mapper, IUnitOfWorkRepository unitOfWorkRepository)
    {
        _mapper = mapper;
        _unitOfWorkRepository = unitOfWorkRepository;
    }
  


   

    [HttpPost]    
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {
        var username = User.GetUserName();
        if (username == createMessageDto.RecipientUsername.ToLower()) return BadRequest("You cannot send messages to yourself");
        var sender = await _unitOfWorkRepository.UserRepository.GetUserByUsernameAsync(username);
        var recipient = await _unitOfWorkRepository.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

        if (recipient == null) return NotFound();        

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content
        };

        _unitOfWorkRepository.MessageRepository.AddMessage(message);

        if (await _unitOfWorkRepository.Complete()) return Ok(_mapper.Map<MessageDto>(message));

        return BadRequest("Failed to send message");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
    {
        messageParams.Username = User.GetUserName();
        var messages = await _unitOfWorkRepository.MessageRepository.GetMessagesForUser(messageParams);
        Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPage);
        return messages;
    }

  

    [HttpDelete("{id}")] 
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var username = User.GetUserName();
        var message = await _unitOfWorkRepository.MessageRepository.GetMessageAsync(id);

        if (message.SenderUsername != username && message.RecipientUsername != username) return Unauthorized();

        if (message.SenderUsername == username) message.SenderDeleted = true;

        if (message.RecipientUsername == username) message.RecipientDeleted = true;

        if (message.SenderDeleted && message.RecipientDeleted) _unitOfWorkRepository.MessageRepository.DeleteMessage(message);

        if (await _unitOfWorkRepository.Complete()) return Ok();

        return BadRequest("Problem deleting the message");
    }


}
