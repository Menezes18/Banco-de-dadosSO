namespace Client
{
    // Classe que representa um comando para o servidor
    public class Command
    {
        public Operacao Op; // Operação a ser executada (Inserir, Atualizar, Remover, Pesquisar)
        public string? Key; // Chave para operações que envolvem chave-valor
        public string? Value; // Valor para operações que envolvem chave-valor
    }

    // Enumeração para as operações possíveis no servidor
    public enum Operacao
    {
        Insert,  // Inserir um novo registro
        Update,  // Atualizar um registro existente
        Remove,  // Remover um registro
        Search   // Pesquisar um registro
    }
}
