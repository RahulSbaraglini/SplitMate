namespace SplitMate.Application.DTOs;

// representa uma transferência que precisa ser feita pra zerar as dívidas do grupo
// FromUserName deve pagar ToUserName o valor Amount
public record DebtResponse(
    Guid FromUserId,
    string FromUserName,
    Guid ToUserId,
    string ToUserName,
    decimal Amount
);