using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Frontend.Models
{
   public class ChatRequest
{
    [Required(ErrorMessage = "You must type a message before sending.")]
    [MinLength(1, ErrorMessage = "The message cannot be empty.")]
    public string? UserMessage { get; set; }
}

}