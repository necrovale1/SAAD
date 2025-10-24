using CommunityToolkit.Mvvm.Messaging.Messages;
using SAAD.Models;

namespace SAAD.Messages
{
         public class AlunoReconhecidoMessage : ValueChangedMessage<User>
        {
            public bool IsEntrada { get; }

            public AlunoReconhecidoMessage(User aluno, bool isEntrada = true) : base(aluno)
            {
                IsEntrada = isEntrada;
            }
         }
}

