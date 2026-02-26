
namespace BambaIba.Application.Abstractions.DomainEvents;

public record NotificationCreatedEvent(
    Guid RecipientUserId,       // Qui doit recevoir la notif ?
    Guid TriggeredByUserId,     // Qui a déclenché l'action ?
    string TriggeredByUsername, // Nom de l'utilisateur (pour éviter une requête DB)
    string MessageType,         // "Like", "Comment", "NewFollower"
    string MessageContent,      // "a aimé votre vidéo", "a commenté..."
    Guid? MediaId,              // Optionnel : ID du média concerné
    string? MediaTitle          // Optionnel : Titre du média
);
