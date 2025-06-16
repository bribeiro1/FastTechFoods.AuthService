namespace FastTechFoods.AuthService.Domain.Constants
{
    public static class UserRoles
    {
        public const string Cliente = "Cliente";
        public const string Atendente = "Atendente";
        public const string Gerente = "Gerente";

        public static readonly HashSet<string> Todos = [Cliente, Atendente, Gerente];
    }
}
