using IdentityMail.Web.Context;
using IdentityMail.Web.DTOs.DraftDtos;
using IdentityMail.Web.DTOs.UserMessageDtos;
using IdentityMail.Web.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityMail.Web.Controllers
{
    public class DraftController(UserManager<AppUser> _userManager, AppDbContext _context) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user= await _userManager.FindByNameAsync(User.Identity.Name);
            if(user is null)
                return RedirectToAction("Login", "Auth");
            

            var draftMessages= await _context.UserMessages.Include(x=> x.Receiver).Where(x=>x.IsDraft && x.SenderId == user.Id && !x.IsDeleted).ToListAsync();
            return View(draftMessages);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user= await _userManager.FindByNameAsync(User.Identity.Name);
            if(user is null)
                return RedirectToAction("Login", "Auth");
            
            var message= await _context.UserMessages.FirstOrDefaultAsync(x=>x.Id == id && x.SenderId == user.Id && x.IsDraft && !x.IsDeleted);
            if(message is null)
                return NotFound();
            
            message.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user= await _userManager.FindByNameAsync(User.Identity.Name);
            if(user is null)
                return RedirectToAction("Login", "Auth");
            
            var message= await _context.UserMessages.Include(x=>x.Receiver).FirstOrDefaultAsync(x=>x.Id == id && x.SenderId == user.Id && x.IsDraft && !x.IsDeleted);
            if(message is null)
                return NotFound();
            
            var draft= new DraftMailDto
            {
                Subject = message.Subject,
                Body = message.Body,
                ReceiverEmail = message.Receiver.Email
            };

            return View(draft);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DraftMailDto draftMailDto)
        {
            var user= await _userManager.FindByNameAsync(User.Identity.Name);
            if(user is null)
                return RedirectToAction("Login", "Auth");
            
            var existingMessage= await _context.UserMessages.FirstOrDefaultAsync(x=>x.Id == draftMailDto.Id && x.SenderId == user.Id && x.IsDraft && !x.IsDeleted);
            if(existingMessage is null)
                return NotFound();

            var receiver= await _userManager.FindByEmailAsync(draftMailDto.ReceiverEmail);
            if(receiver is null)
            {
                ModelState.AddModelError(string.Empty, "Alıcı bulunamadı.");
                return View(draftMailDto);
            }

            existingMessage.Subject = draftMailDto.Subject;
            existingMessage.Body = draftMailDto.Body;
            existingMessage.ReceiverId = receiver.Id;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
