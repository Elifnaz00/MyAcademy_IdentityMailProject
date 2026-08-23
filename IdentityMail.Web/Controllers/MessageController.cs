using IdentityMail.Web.Context;
using IdentityMail.Web.DTOs.UserMessageDtos;
using IdentityMail.Web.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using PagedList.Core;

namespace IdentityMail.Web.Controllers
{

    public class MessageController(UserManager<AppUser> _userManager, AppDbContext _context) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index(bool? isRead, bool? isImportant, string? sortOrder, string search, int page=1, int pageSize=6)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if(user is null)
                return NotFound();

            ViewBag.FullName = $"{user.FirstName} {user.LastName}";

            var messageList =  _context.UserMessages
                .Include(a => a.Sender)
                .Where(m => m.ReceiverId == user.Id && m.IsDeleted == false);
                
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

            return View(model);
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



        public async Task<IActionResult> MailDetail(int id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user is null)
                return NotFound();
            
            var message = await _context.UserMessages.Include(x => x.Sender).FirstOrDefaultAsync(x => x.Id == id && x.ReceiverId == user.Id);
            if (message == null)
                return NotFound();

            message.IsRead = true;
            await _context.SaveChangesAsync();

            return View(message);
        }



        public async Task<IActionResult> SentMessages()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user is null)
                return NotFound();
            

            var sentMessageList = await _context.UserMessages.Include(x => x.Sender).Where(x => x.SenderId == user.Id && x.IsDeleted == false).ToListAsync();

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

            var importantMessageList = await _context.UserMessages.Where(x => x.IsImportant == true && x.ReceiverId == user.Id && x.IsDeleted == false).ToListAsync();
            return View(importantMessageList);

        }



        //çöp kutusuna taşıma işlemi
        public async Task<IActionResult> MoveToTrash(int id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if(user is null)
                return NotFound();

            var message = await _context.UserMessages.FirstOrDefaultAsync(x => x.Id == id && x.ReceiverId == user.Id);
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

            var message = await _context.UserMessages.FirstOrDefaultAsync(x => x.Id == id && x.ReceiverId == user.Id);
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

            var trashMessageList = await _context.UserMessages.Include(a => a.Sender).Where(x => x.ReceiverId == user.Id && x.IsDeleted == true).ToListAsync();

            return View(trashMessageList);

        }

        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if(user is null) return NotFound();

            var deleteMessage = await _context.UserMessages.FirstOrDefaultAsync(x => x.Id == id && x.ReceiverId == user.Id);
            if (deleteMessage is null)
                return NotFound();

            _context.UserMessages.Remove(deleteMessage);
            return RedirectToAction(nameof(Index));
        }




    }
}
