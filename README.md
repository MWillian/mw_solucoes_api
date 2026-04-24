# MW Solucoes API

API da plataforma MW Soluções, focada em gerenciamento de solicitações de serviço e catálogo de serviços de manutenção de microcomputadores.

## Visão do produto
Uma plataforma simples e confiável para conectar clientes a serviços de manutenção, com rastreio de status e operação clara para técnicos.

## Para quem é
- Cliente: abre solicitações, acompanha o andamento e consulta serviços disponíveis.
- Técnico: controla o ciclo de vida das solicitações e mantém o catálogo de serviços.

## Jornadas principais
- Cliente cria solicitação com serviços, acompanha status e histórico.
- Técnico aceita, atualiza, finaliza ou cancela solicitações conforme o fluxo.
- Catálogo de serviços permite ativar/desativar serviços conforme disponibilidade.

## O que temos atualmente
- API organizada em camadas (Domain, Application, Infrastructure, Communication, API).
- Entidades e regras de negócio para solicitações de serviço, usuários e serviços de manutenção.
- Casos de uso para criar, atualizar, listar, aceitar, rejeitar, finalizar, cancelar e remover solicitações.
- Endpoints para catálogo de serviços de manutenção (listar, obter por ID, criar, atualizar, ativar/desativar e remover).
- Autenticação/autorização por roles (Cliente/Técnico) aplicada em endpoints sensíveis.
- Base para persistência e migrações no projeto de infraestrutura.

## O que já foi construído
- Estrutura da solução com projetos separados por responsabilidade.
- Mapeamentos e contratos de requisições/respostas para comunicação entre API e front-end.
- Regras de status para solicitações (Criada, Em Progresso, Finalizada, Cancelada, Rejeitada).

## Ferramentas utilizadas
- .NET / ASP.NET Core
- Entity Framework Core
- C#
- PostgreSQL

## Etapa atual
Primeira etapa do back-end concluída. O próximo passo é iniciar o desenvolvimento do front-end, com implementação em TypeScript e Angular.

## O que ainda virá
- Iniciar o front-end com base em Angular.
- Refinar documentação de endpoints conforme o front-end evoluir.
- Implementar a extensão do back-end, com lógica de pagamento e aperfeiçoamento do fluxo das solicitações de serviço.
- Containerização da aplicação em uma VPS própria.
- Aquisição de domínio próprio para disponibilidade da solução.