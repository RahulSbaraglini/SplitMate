namespace SplitMate.Application.DTOs;

// DTO de entrada — representa exatamente o que o usuário manda no corpo da requisição
// usamos record porque é imutável por padrão e tem menos boilerplate que uma classe
// Clean Code — o nome RegisterRequest deixa claro que é uma entrada de dados
public record RegisterRequest(string Name, string Email, string Password);