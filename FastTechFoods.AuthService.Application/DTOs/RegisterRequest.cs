﻿namespace FastTechFoods.AuthService.Application.DTOs
{
    public class RegisterRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Cpf { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
