using IdentityMail.Web.Context;
using IdentityMail.Web.DTOs.UserMessageDtos;
using IdentityMail.Web.Entities;
using IdentityMail.Web.Migrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using PagedList.Core;

namespace IdentityMail.Web.Controllers
{

    public class MessageController(UserManager<AppUser> _userManager, AppDbContext _context) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index(bool? isRead, bool? isImportant, string? sortOrder, string search, int page=1, int pageSize=3)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if(user is null)
                return NotFound();

            ViewBag.FullName = $"{user.FirstName} {user.LastName}";

            var messageList =  _context.UserMessages
                .Include(a => a.Sender).Include(a => a.Category)
                .Where(m => m.ReceiverId == user.Id && !m.IsDeleted && !m.IsDraft);
                
            if(!string.IsNullOrEmpty(search))
            {
                messageList= messageList.Where(m=>m.Subject.Contains(search) || m.Body.Contains(search) || m.Category.Name.Contains(search) || m.Sender.FirstName.Contains(search));  
            }
            if(!string.IsNullOrEmpty(sortOrder))
            {
                switch(sortOrder)
                {
                    case "true": 
                        messageList = messageList.OrderByDescending(m => m.SendDate);
                        break;

                    case "false":
                        messageList = messageList.OrderBy(m => m.SendDate);
                        break;

                }

            }
            if(isRead.HasValue)
            {
                messageList = messageList.Where(m => m.IsRead == isRead.Value);
            }

            if(isImportant.HasValue)
            {
                messageList = messageList.Where(m => m.IsImportant == isImportant.Value);
            }


            messageList = messageList.OrderByDescending(m => m.SendDate);
            PagedList<UserMessage> model = new PagedList<UserMessage>(messageList, page, pageSize);

            var notIsReadMessageList= await _context.UserMessages.CountAsync(m => m.ReceiverId == user.Id && m.IsRead == false && !m.IsDeleted && !m.IsDraft);
            ViewBag.NotIsReadMessages = notIsReadMessageList;

            ViewBag.Categories = await _context.Categories
            .Where(a => a.IsActive)
            .Select(a => new SelectListItem
            {
                Text = a.Name,
                Value = a.Id.ToString()
            })
            .ToListAsync();

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> MessageCategoryAssign(int categoryId, int messageId)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var message = await _context.UserMessages.FirstOrDefaultAsync(x => x.ReceiverId == user.Id && x.Id == messageId);
            if(message is null)
            {
                return NotFound();
            }   

            message.CategoryId = categoryId;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> SendMail()
        {

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> SendMail(SendMailDto sendMailDto)
        {
            var sender = await _userManager.FindByNameAsync(User.Identity.Name);
            var receiver = await _userManager.FindByEmailAsync(sendMailDto.ReceiverEmail);
            if (receiver is null)
            {
                ModelState.AddModelError(string.Empty, "Girdiğiniz Mail ile sistemde kayıtlı kullanıcı bulunamadı.");
                return View(sendMailDto);
            }
            var newMessage = new UserMessage
            {
                SenderId = sender.Id,
                ReceiverId = receiver.Id,
                Subject = sendMailDto.Subject,
                SendDate = DateTime.UtcNow,
                Body = sendMailDto.Body,
            };
            _context.UserMessages.Add(newMessage);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> MailDetail(int id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user is null)
                return NotFound();
            
            var message = await _context.UserMessages.Include(x => x.Sender).FirstOrDefaultAsync(x => x.Id == id && x.ReceiverId == user.Id);

            if (message == null)
                return NotFound();


            if (!message.IsRead)
            {
                message.IsRead = true;
                await _context.SaveChangesAsync();
            }

            var messageList = await _context.UserMessages.Where(x => x.ReplyToMessageId == message.Id)
              .Include(x => x.Sender)
              .ToListAsync();

            var mailDetailDto= new MailDetailDto
            {
                UserMessage = message,
                Messages = messageList
            };

            return View(mailDetailDto);
        }



        [HttpPost]
        public async Task<IActionResult> Reply(UserMessage userMessage)
        {
            var user= await _userManager.FindByNameAsync(User.Identity.Name);
            if(user is null)
                return NotFound();

            var reciever = await _userManager.FindByIdAsync(userMessage.ReceiverId.ToString());
            if(reciever is null)
            {
                ModelState.AddModelError("", "Alıcı kullanıcı sisteme kayıtlı değil.");

                var message = await _context.UserMessages.Include(x => x.Sender).Include(x => x.ReplyToMessage)
                             .ThenInclude(x => x.Sender).FirstOrDefaultAsync(x => x.Id == userMessage.ReplyToMessageId && x.ReceiverId == user.Id);

                if(message == null) return NotFound();

                var messageList = await _context.UserMessages
               .Where(x => x.ReplyToMessageId == message.Id)
               .Include(x => x.Sender)
               .OrderBy(x => x.SendDate)
               .ToListAsync();

           
                var mailDetailDto = new MailDetailDto
                {
                    UserMessage = message,
                    Messages = messageList
                };

                return View("MailDetail", mailDetailDto);

            }

            var replyMessage= new UserMessage
            { 
                SenderId = user.Id,
                ReceiverId = userMessage.ReceiverId,
                Subject = userMessage.Subject,
                Body = userMessage.Body,
                SendDate = DateTime.UtcNow,
                ReplyToMessageId = userMessage.ReplyToMessageId
            };
            _context.UserMessages.Add(replyMessage);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(MailDetail), new {id = userMessage.ReplyToMessageId});
        }



        public async Task<IActionResult> SentMessages()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user is null)
                return NotFound();
            

            var sentMessageList = await _context.UserMessages.Include(x => x.Receiver).Where(x => x.SenderId == user.Id && !x.IsDeleted && !x.IsDraft).ToListAsync();

            return View(sentMessageList);

        }


        //önemli olarak işaretleme işlemi
        public async Task<IActionResult> ToggleImportant(int id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user is null)
                return NotFound();
            
            var message = await _context.UserMessages.FirstOrDefaultAsync(x => x.Id == id && x.ReceiverId == user.Id);
            if (message is null)
                return NotFound();

            message.IsImportant = !message.IsImportant;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        //önemli mesajlar sayfası
        public async Task<IActionResult> ImportantMessages()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if(user is null)
                return NotFound();

            var importantMessageList = await _context.UserMessages.Include(x=> x.Sender).Where(x => x.IsImportant == true && x.ReceiverId == user.Id && x.IsDeleted == false && x.IsDraft== false).ToListAsync();
            return View(importantMessageList);

        }



        //çöp kutusuna taşıma işlemi
        public async Task<IActionResult> MoveToTrash(int id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if(user is null)
                return NotFound();

            var message = await _context.UserMessages.FindAsync(id);
            if (message == null)
                return NotFound();


            message.IsDeleted = true;
         
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //çöpten geri getirme işlemi
        public async Task<IActionResult> Restore(int id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if(user is null)
                return NotFound();  

            var message = await _context.UserMessages.FirstOrDefaultAsync(x => x.Id == id && x.ReceiverId == user.Id && x.IsDeleted);
            if (message == null)
                return NotFound();

            message.IsDeleted = false;
           
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Trash));

        }

        //çöp kutusu
        public async Task<IActionResult> Trash()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if(user is null) return NotFound(); 

            var trashMessageList = await _context.UserMessages.Include(a => a.Sender).Where(x => x.ReceiverId == user.Id  && x.IsDeleted).ToListAsync();

            return View(trashMessageList);

        }


      
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if(user is null) return NotFound();

            var deleteMessage = await _context.UserMessages.FindAsync(id);
            if (deleteMessage is null)
                return NotFound();
           
            _context.UserMessages.Remove(deleteMessage);
            await _context.SaveChangesAsync();

            return RedirectToAction("Trash","Message");
        }

        //mesajı şikayet etme işlemi
        public async Task<IActionResult> Report(int id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if(user is null) return NotFound();

            var message = await _context.UserMessages.FirstOrDefaultAsync(x => x.Id == id && x.ReceiverId == user.Id && !x.IsDeleted);
            if (message is null)
                return NotFound();

            message.IsComplaint = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }




    }
}
