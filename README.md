# MW Soluções API

API da plataforma MW Soluções, focada no gerenciamento de solicitações de serviço e na manutenção de um catálogo dinâmico de serviços para microcomputadores. Construída sob os princípios do Domain-Driven Design.

## Visão do Produto

Uma plataforma robusta e confiável para conectar clientes a serviços de manutenção, oferecendo rastreabilidade total de status para os clientes e uma operação centralizada e clara para os técnicos.

## Perfis de Acesso

* **Cliente:** Abre solicitações de serviço, acompanha o andamento via linha do tempo e consulta os serviços disponíveis no catálogo.
* **Técnico:** Controla o ciclo de vida das solicitações (aceite, atualização, finalização) e realiza a gestão integral do catálogo de serviços.

## Arquitetura e Segurança

A aplicação foi desenvolvida em camadas (Domain, Application, Infrastructure, Communication, API, Exception).

* **Autenticação e Autorização:** Autenticação robusta via JWT configurado para ciclo de vida curto, acompanhado de implementação completa de *Refresh Tokens* com rotação e revogação no banco de dados.
* **Segurança em Nível de Negócio:** Proteções contra BOLA (Broken Object Level Authorization) e BFLA (Broken Function Level Authorization) implementadas diretamente nas regras da camada de Domínio, garantindo que usuários só acessem e manipulem recursos que lhes pertencem ou competem ao seu cargo.
* **Defesa de Borda:** Middleware global de *Rate Limiting* com políticas segmentadas (ex: limitação estrita nas rotas de autenticação contra *brute-force*), além da configuração de cabeçalhos de segurança HTTP rigorosos (CSP, HSTS, X-Frame-Options, X-Content-Type-Options).
* **Background Jobs:** Implementação de um *Worker Service* rodando em segundo plano para a manutenção preventiva do banco de dados, realizando a limpeza periódica de *Refresh Tokens* expirados.
* **Observabilidade:** Configuração de logs estruturados utilizando Serilog, mantendo o registro monitorado de exceções globais e operações críticas no sistema.

## Funcionalidades em Destaque

* **Gestão de Solicitações de Serviço:** Ciclo de vida completo com validações de transição de status (Criada, Em Progresso, Finalizada, Cancelada, Rejeitada).
* **Histórico e Rastreabilidade:** Tabela de histórico imutável vinculada ao Domínio, gerando uma linha do tempo automática e segura a cada mudança de estado das solicitações.
* **Catálogo de Serviços Inteligente:** Endpoints de gerenciamento (CRUD) com visibilidade condicional baseada em perfil.

## Stack Tecnológica

* C# / .NET 10 / ASP.NET
* Entity Framework Core
* PostgreSQL
* Serilog (Structured Logging)

## Próximos Passos

A primeira fase do back-end, contemplando as regras de negócio core e o pilar de segurança, está concluída. Os próximos passos do ciclo de desenvolvimento incluem:

* Construção do front-end utilizando o framework Angular.
* Implementação de um módulo de integração com API de pagamentos.
* Containerização da aplicação utilizando Docker.
* Configuração e deploy em VPS com aquisição de domínio próprio.