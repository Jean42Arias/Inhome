using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SalesSystem.Areas.Customers.Models
{
    public class InputModelRegister : TReports_clients
    {
        [Required(ErrorMessage = "El campo nid es obligatorio.")]
        public string Nid { set; get; }
        [Required(ErrorMessage = "El campo nombre es obligatorio.")]
        public string Name { set; get; }
        [Required(ErrorMessage = "El campo apellido es obligatorio.")]
        public string LastName { set; get; }
        [Required(ErrorMessage = "El campo email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es una dirección de correo electrónico válida.")]
        public string Email { set; get; }
        [Required(ErrorMessage = "El campo direccion es obligatorio.")]
        public string Direction { set; get; }
        [Required(ErrorMessage = "El campo telefono es obligatorio.")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^(?:\+?1[-. ]?)?\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "El formato telefono ingresado no es válido.")]
        public string Phone { set; get; }
        [Required(ErrorMessage = "El campo fecha es obligatorio.")]
        [DataType(DataType.Date)]
        public DateTime Date { set; get; }
        public bool Credit { set; get; }
        public byte[] Image { get; set; }
        public int IdClient { set; get; }
        [TempData]
        public string ErrorMessage { get; set; }
    }
}
