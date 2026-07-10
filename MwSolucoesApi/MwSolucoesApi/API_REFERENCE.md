# Documentação da API - Referência de Endpoints

## Módulo: Autenticação (`/api/Auth`)

Este módulo gerencia o controle de acesso à plataforma MW Soluções. Ele implementa o padrão de segurança **JWT (JSON Web Token)** para sessões curtas e **Refresh Tokens via Cookies HttpOnly** para sessões seguras e duradouras. Além disso, os endpoints estão protegidos por regras de *Rate Limiting* para evitar ataques de força bruta.

---

### 1. Realizar Login

Autentica o usuário na plataforma. Se as credenciais estiverem corretas, a API retorna o JWT no corpo da requisição e injeta um Cookie seguro (`HttpOnly`) no navegador contendo o Refresh Token.

* **Método:** `POST`
* **Rota:** `/api/Auth/login`
* **Autenticação:** Nenhuma (`AllowAnonymous`)
* **Proteção:** Rate Limiting (`auth`)

**Corpo da Requisição (JSON)**

```json
{
  "email": "tecnico@mwsolucoes.com",
  "password": "SenhaSegura123!"
}

```

**Respostas Esperadas**

* `200 OK`: Login efetuado. Retorna o `Token` (JWT) e injeta o Cookie `refreshToken` (expira em 7 dias).
* `400 Bad Request`: Erro de validação nos campos.
* `401 Unauthorized`: E-mail ou senha incorretos.
* `404 Not Found`: Usuário não cadastrado.

---

### 2. Renovar Sessão (Refresh Token)

Gera um novo Token JWT sem exigir que o usuário digite a senha novamente. A API lê automaticamente o Cookie `refreshToken` injetado pelo endpoint de login.

* **Método:** `POST`
* **Rota:** `/api/Auth/refresh`
* **Autenticação:** Nenhuma (`AllowAnonymous` - Baseado em Cookie)
* **Proteção:** Rate Limiting (`auth`)

**Respostas Esperadas**

* `200 OK`: Sessão renovada. Retorna o novo `Token` (JWT) e atualiza o Cookie com um novo Refresh Token.
* `400 Bad Request`: Cookie de Refresh Token ausente na requisição.
* `401 Unauthorized`: Refresh Token inválido ou expirado.

---

### 3. Encerrar Sessão (Logout)

Invalida o Refresh Token atual no banco de dados e limpa o Cookie armazenado no navegador do usuário.

* **Método:** `POST`
* **Rota:** `/api/Auth/logout`
* **Autenticação:** Requer Token JWT (`Authorization: Bearer <token>`)

**Respostas Esperadas**

* `204 No Content`: Logout realizado com sucesso e Cookie excluído.
* `401 Unauthorized`: Token JWT ausente ou inválido.

---

### 4. Solicitar Recuperação de Senha (Esqueci minha senha)

Inicia o fluxo de recuperação de senha. Para evitar ataques de enumeração (onde hackers descobrem quem é cliente tentando e-mails aleatórios), a API **sempre retorna a mesma mensagem de sucesso**, independentemente de o e-mail existir ou não na base de dados.

* **Método:** `PUT`
* **Rota:** `/api/Auth/forgot-password`
* **Autenticação:** Nenhuma (`AllowAnonymous`)
* **Proteção:** Rate Limiting (`auth`)

**Corpo da Requisição (JSON)**

```json
{
  "email": "cliente@email.com"
}

```

**Respostas Esperadas**

* `200 OK`: Solicitação recebida. Se o e-mail for válido, um link de recuperação será enviado via e-mail (Resend).

---

### 5. Redefinir Senha (Reset)

Conclui o fluxo de recuperação utilizando o token de segurança enviado para o e-mail do usuário.

* **Método:** `POST`
* **Rota:** `/api/Auth/reset-password`
* **Autenticação:** Nenhuma (`AllowAnonymous`)
* **Proteção:** Rate Limiting (`auth`)

**Corpo da Requisição (JSON)**

```json
{
  "token": "dGVzdGUtdG9rZW4tc2VndXJhbmNh...",
  "newPassword": "NovaSenhaForte123!"
}

```

**Respostas Esperadas**

* `200 OK`: Senha redefinida com sucesso.
* `400 Bad Request`: Token inválido, expirado, ou a nova senha não atinge os critérios de segurança mínimos (6 caracteres).

---

### 6. Alterar Senha Atual (Perfil do Usuário)

Permite que o usuário logado altere sua própria senha, exigindo a confirmação da senha atual por segurança.

* **Método:** `PUT`
* **Rota:** `/api/Auth/update-password/me`
* **Autenticação:** Requer Token JWT (`Authorization: Bearer <token>`)
* **Proteção:** Rate Limiting (`auth`)

**Corpo da Requisição (JSON)**

```json
{
  "currentPassword": "SenhaAntiga123",
  "newPassword": "NovaSenhaMaisForte456",
  "confirmNewPassword": "NovaSenhaMaisForte456"
}

```

**Respostas Esperadas**

* `204 No Content`: Senha atualizada com sucesso.
* `400 Bad Request`: Validação de senhas incompatíveis (A confirmação está diferente da nova senha) ou regras de segurança não atendidas.
* `401 Unauthorized`: A `currentPassword` (senha atual) fornecida está incorreta.
--- 

## Módulo: Usuários (`/api/User`)

Este módulo gerencia as contas de usuários da plataforma (Clientes e Técnicos/Admins). Contempla desde o registro público de novos clientes até o gerenciamento administrativo completo, seguindo as diretrizes de segurança BOLA (acessos `/me` restritos ao próprio usuário) e BFLA (acessos administrativos).

---

### 1. Criar Usuário (Registro)

Cria uma nova conta de usuário no sistema. Geralmente utilizado na tela de cadastro público de clientes.

* **Método:** `POST`
* **Rota:** `/api/User`
* **Autenticação:** Nenhuma
* **Rate Limit:** Aplicado (Política `auth` - previne criação em massa/spam)

**Corpo da Requisição (JSON)**
*(Exemplo inferido das entidades do domínio)*

```json
{
  "name": "Nome Completo do Usuário",
  "email": "usuario@email.com",
  "password": "SenhaSegura123!",
  "cpf": "12345678901",
  "phoneNumber": "11999999999",
  "address": {
    "logradouro": "Rua Exemplo",
    "numero": "123",
    "bairro": "Centro",
    "cidade": "São Paulo",
    "estado": "SP",
    "cep": "01000000"
  }
}

```

**Respostas Esperadas**

* `201 Created`: Usuário criado com sucesso.
* **Corpo:** Retorna os dados do usuário recém-criado (sem a senha).


* `400 Bad Request`: Erro de validação nos dados enviados (ex: CPF inválido, e-mail já em uso ou campos obrigatórios ausentes).

---

### 2. Obter Meu Perfil

Retorna as informações da conta do próprio usuário que está realizando a requisição, baseado no Token JWT enviado.

* **Método:** `GET`
* **Rota:** `/api/User/me`
* **Autenticação:** Requer Token JWT (`Authorization: Bearer <token>`)

**Corpo da Requisição**

* Vazio.

**Respostas Esperadas**

* `200 OK`: Retorna os dados do perfil do usuário.
* **Corpo:**
```json
{
  "id": "beed9cdc-1280...",
  "name": "Nome do Usuário",
  "email": "usuario@email.com",
  "phoneNumber": "11999999999",
  "role": "Cliente",
  "isActive": true
}

```




* `401 Unauthorized`: Token ausente ou inválido.
* `404 Not Found`: Usuário não encontrado no banco de dados.

---

### 3. Atualizar Meu Perfil

Atualiza as informações de cadastro do próprio usuário autenticado.

* **Método:** `PUT`
* **Rota:** `/api/User/me`
* **Autenticação:** Requer Token JWT (`Authorization: Bearer <token>`)

**Corpo da Requisição (JSON)**

```json
{
  "name": "Nome Atualizado",
  "phoneNumber": "11988888888",
  "address": {
    "logradouro": "Nova Rua",
    "numero": "456",
    "bairro": "Novo Bairro",
    "cidade": "São Paulo",
    "estado": "SP",
    "cep": "02000000"
  }
}

```

**Respostas Esperadas**

* `200 OK`: Perfil atualizado com sucesso (retorna o DTO do usuário atualizado).
* `400 Bad Request`: Erro de validação nos campos informados.
* `401 Unauthorized`: Token ausente ou inválido.
* `404 Not Found`: Usuário não encontrado.

---

### 4. Desativar Minha Conta

Permite que o usuário autenticado inative sua própria conta no sistema. O registro não é apagado fisicamente (Soft Delete) para manter a integridade dos históricos de serviços.

* **Método:** `DELETE`
* **Rota:** `/api/User/me`
* **Autenticação:** Requer Token JWT (`Authorization: Bearer <token>`)

**Corpo da Requisição**

* Vazio.

**Respostas Esperadas**

* `204 No Content`: Conta desativada com sucesso.
* `401 Unauthorized`: Token ausente ou inválido.
* `404 Not Found`: Usuário não encontrado.

---

### 5. Obter Usuário Específico (Administrador)

Busca os dados de qualquer usuário cadastrado no sistema informando o ID. Rota protegida por política de autorização.

* **Método:** `GET`
* **Rota:** `/api/User/{id}`
* **Autenticação:** Requer Token JWT com privilégio de Admin (`Policy: AdminAccess`)

**Parâmetros de Rota**

* `id` (GUID): O identificador único do usuário.

**Respostas Esperadas**

* `200 OK`: Retorna os dados do usuário.
* `401 / 403`: Falta de autenticação ou usuário autenticado não possui privilégio administrativo.
* `404 Not Found`: Usuário não encontrado.

---

### 6. Listar Usuários (Administrador)

Retorna uma lista paginada de todos os usuários do sistema, permitindo a aplicação de filtros (Nome, E-mail, Status, etc.).

* **Método:** `GET`
* **Rota:** `/api/User`
* **Autenticação:** Requer Token JWT com privilégio de Admin (`Policy: AdminAccess`)

**Parâmetros de Busca (Query Strings)**
*(Inferido com base em `UserFilters`)*

* `name` (string, opcional): Filtra por nome.
* `email` (string, opcional): Filtra por e-mail.
* `isActive` (bool, opcional): Filtra por status de ativação.
* `page` (int, padrão: 1): Número da página.
* `pageSize` (int, padrão: 20): Quantidade de registros por página.

**Respostas Esperadas**

* `200 OK`: Lista paginada retornada com sucesso.
* **Corpo:**
```json
{
  "items": [
    { "id": "...", "name": "Usuario 1", "email": "..." },
    { "id": "...", "name": "Usuario 2", "email": "..." }
  ],
  "totalRecords": 2,
  "currentPage": 1,
  "pageSize": 20
}

```




* `401 / 403`: Acesso negado (não autenticado ou sem permissão de Admin).

---

### 7. Desativar Usuário (Administrador)

Permite que um Administrador inative (bloqueie) a conta de qualquer usuário do sistema.

* **Método:** `PATCH`
* **Rota:** `/api/User/deactivate/{id}`
* **Autenticação:** Requer Token JWT com privilégio de Admin (`Policy: AdminAccess`)

**Parâmetros de Rota**

* `id` (GUID): O identificador único do usuário a ser desativado.

**Respostas Esperadas**

* `204 No Content`: Usuário desativado com sucesso.
* `401 / 403`: Acesso negado.
* `404 Not Found`: Usuário não encontrado.

---

### 8. Reativar Usuário (Administrador)

Permite que um Administrador reative uma conta de usuário previamente desativada.

* **Método:** `PATCH`
* **Rota:** `/api/User/activate/{id}`
* **Autenticação:** Requer Token JWT com privilégio de Admin (`Policy: AdminAccess`)

**Parâmetros de Rota**

* `id` (GUID): O identificador único do usuário a ser reativado.

**Respostas Esperadas**

* `204 No Content`: Usuário reativado com sucesso.
* `401 / 403`: Acesso negado.
* `404 Not Found`: Usuário não encontrado.

--- 


## Módulo: Catálogo de Serviços de Manutenção (`/api/MaintenanceServices`)

Este módulo é responsável pelo gerenciamento do catálogo de serviços oferecidos pela plataforma. Possui regras estritas de visibilidade de dados: **Clientes** têm acesso apenas à leitura dos serviços ativos, enquanto **Técnicos** possuem controle total do ciclo de vida (CRUD) e visualização global do catálogo (ativos e inativos).

---

### 1. Listar Serviços de Manutenção

Retorna uma lista paginada do catálogo de serviços. A resposta varia de acordo com o perfil de acesso: clientes recebem apenas serviços com status `isActive: true`, enquanto técnicos recebem o catálogo completo, podendo aplicar o filtro desejado.

* **Método:** `GET`
* **Rota:** `/api/MaintenanceServices`
* **Autenticação:** Requer Token JWT (`Authorization: Bearer <token>`)

**Parâmetros de Busca (Query Strings)**

* `name` (string, opcional): Busca parcial pelo nome do serviço.
* `price` (decimal, opcional): Filtra pelo preço exato.
* `isActive` (bool, opcional): Filtra por status (ignorado caso o usuário seja Cliente, forçando `true`).
* `category` (int, opcional): Filtra pela categoria do serviço (Enum).
* `page` (int, padrão: 1): Número da página.
* `pageSize` (int, padrão: 20): Quantidade de registros por página.
* `sortBy` / `sortDirection` (string): Ordenação padrão por `name` em `asc`.

**Respostas Esperadas**

* `200 OK`: Lista paginada retornada com sucesso.
* **Corpo:**
```json
{
  "items": [
    {
      "id": 1,
      "name": "Troca de Tela de Notebook",
      "description": "Substituição completa do display LCD/LED.",
      "price": 350.00,
      "category": 1,
      "isActive": true
    }
  ],
  "totalRecords": 1,
  "currentPage": 1,
  "pageSize": 20
}

```

* `401 Unauthorized`: Token ausente ou inválido.

---

### 2. Obter Serviço Específico por ID

Retorna os detalhes de um serviço de manutenção específico.

* **Método:** `GET`
* **Rota:** `/api/MaintenanceServices/{id}`
* **Autenticação:** Requer Token JWT (`Authorization: Bearer <token>`)

**Parâmetros de Rota**

* `id` (int): O identificador único do serviço.

**Respostas Esperadas**

* `200 OK`: Retorna os dados do serviço.
* `401 Unauthorized`: Token ausente ou inválido.
* `404 Not Found`: Serviço não encontrado, **ou** o serviço está inativo e a requisição foi feita por um Cliente (proteção de dados).

---

### 3. Criar Novo Serviço (Restrito)

Adiciona um novo serviço de manutenção ao catálogo da plataforma. O serviço é criado ativado por padrão.

* **Método:** `POST`
* **Rota:** `/api/MaintenanceServices`
* **Autenticação:** Requer Token JWT com privilégio Técnico (`Policy: Technician`)

**Corpo da Requisição (JSON)**

```json
{
  "name": "Formatação com Backup",
  "description": "Instalação limpa do SO preservando os arquivos do cliente.",
  "price": 150.00,
  "category": 2
}

```

**Respostas Esperadas**

* `201 Created`: Serviço criado com sucesso (retorna o objeto criado no corpo).
* `400 / 422`: Erro de validação (ex: preço negativo, nome muito curto).
* `403 Forbidden`: Acesso negado. O usuário não possui cargo de Técnico.
* `409 Conflict`: Já existe um serviço cadastrado com este nome.

---

### 4. Atualizar Serviço (Restrito)

Atualiza as informações de um serviço de manutenção existente. Valida conflitos de nomenclatura ignorando o próprio registro.

* **Método:** `PUT`
* **Rota:** `/api/MaintenanceServices/{id}`
* **Autenticação:** Requer Token JWT com privilégio Técnico (`Policy: Technician`)

**Corpo da Requisição (JSON)**

```json
{
  "name": "Formatação com Backup",
  "description": "Nova descrição para o serviço.",
  "price": 180.00,
  "category": 2
}

```

**Respostas Esperadas**

* `200 OK`: Serviço atualizado com sucesso.
* `400 / 422`: Erro de validação nos campos informados.
* `403 Forbidden`: Acesso negado. O usuário não possui cargo de Técnico.
* `404 Not Found`: Serviço não encontrado no banco de dados.
* `409 Conflict`: O nome escolhido já está sendo utilizado por outro serviço.

---

### 5. Desativar Serviço (Restrito)

Inativa um serviço no catálogo. Ele deixará de ser visível para os Clientes, impedindo que novas solicitações sejam abertas com ele, mas mantém o histórico íntegro.

* **Método:** `PATCH`
* **Rota:** `/api/MaintenanceServices/{id}/deactivate`
* **Autenticação:** Requer Token JWT com privilégio Técnico (`Policy: Technician`)

**Respostas Esperadas**

* `204 No Content`: Serviço desativado com sucesso.
* `400 Bad Request`: Serviço já se encontra desativado.
* `403 Forbidden`: Acesso negado.
* `404 Not Found`: Serviço não encontrado.

---

### 6. Reativar Serviço (Restrito)

Reativa um serviço previamente inativado no catálogo, tornando-o novamente visível e selecionável para os Clientes.

* **Método:** `PATCH`
* **Rota:** `/api/MaintenanceServices/{id}/reactivate`
* **Autenticação:** Requer Token JWT com privilégio Técnico (`Policy: Technician`)

**Respostas Esperadas**

* `204 No Content`: Serviço reativado com sucesso.
* `400 Bad Request`: Serviço já se encontra ativo.
* `403 Forbidden`: Acesso negado.
* `404 Not Found`: Serviço não encontrado.

---

### 7. Excluir Serviço Permanentemente (Restrito)

Remove permanentemente o registro do catálogo (Hard Delete). *Nota: Idealmente recomendado apenas para serviços recém-criados por engano, preferindo-se a Desativação para serviços que já compõem histórico de ordens passadas.*

* **Método:** `DELETE`
* **Rota:** `/api/MaintenanceServices/{id}`
* **Autenticação:** Requer Token JWT com privilégio Técnico (`Policy: Technician`)

**Respostas Esperadas**

* `204 No Content`: Serviço deletado com sucesso.
* `403 Forbidden`: Acesso negado.
* `404 Not Found`: Serviço não encontrado.

---


## Módulo: Solicitações de Serviço (`/api/ServiceRequest`)

Este módulo gerencia o coração da plataforma: o ciclo de vida das ordens de serviço (OS) de manutenção. Ele orquestra a comunicação entre Clientes (que abrem os chamados) e Técnicos (que assumem, diagnosticam e finalizam os serviços). O fluxo garante o rastreio rigoroso do histórico, geração de documentos em PDF e disparos de notificações por e-mail, mantendo a proteção de dados (BOLA) entre os diferentes perfis.

---

### 1. Criar Solicitação de Serviço (Fase 1: Abertura)

Abre uma nova ordem de serviço na plataforma vinculada ao cliente autenticado. A solicitação entra automaticamente no status inicial (`Created`). Neste momento, o cliente informa apenas o aparelho e o sintoma.

* **Método:** `POST`
* **Rota:** `/api/ServiceRequest`
* **Autenticação:** Requer Token JWT (`Authorization: Bearer <token>`)

**Corpo da Requisição (JSON)**

```json
{
  "equipmentType": 1, 
  "brandModel": "Notebook Dell Inspiron 15",
  "reportedProblem": "A tela está piscando e o teclado parou de funcionar."
}

```

**Respostas Esperadas**

* `201 Created`: Serviço criado com sucesso. Retorna os dados da solicitação recém-gerada, incluindo o ID e o Número de Protocolo.
* `400 / 422`: Erro de validação (ex: modelo ou problema em branco).
* `401 Unauthorized`: Token ausente ou inválido.
* `409 Conflict`: Conflito na geração do protocolo (Tratado automaticamente por retry interno).

---

### 2. Listar Minhas Solicitações (Cliente / Pessoal)

Retorna uma lista paginada de solicitações de serviço pertencentes **exclusivamente** ao usuário autenticado (BOLA aplicado).

* **Método:** `GET`
* **Rota:** `/api/ServiceRequest`
* **Autenticação:** Requer Token JWT (`Authorization: Bearer <token>`)

**Parâmetros de Busca (Query Strings)**

* `status` (int, opcional): Filtra pelo status do serviço (ex: 0 = Created, 1 = InProgress, 2 = Finished).
* `protocol` (string, opcional): Busca pelo número do protocolo gerado.
* `createdAt` (date, opcional): Filtra pela data de abertura.
* `equipmentType` (int, opcional): Filtra por tipo de equipamento.
* `partsCost` (decimal, opcional): Filtra pelo custo de peças.
* `page` / `pageSize` (int): Controle de paginação (Padrão: 1 e 20).
* `sortBy` / `sortDirection`: Ordenação (Padrão: `createdAt` desc).

**Respostas Esperadas**

* `200 OK`: Lista paginada retornada com sucesso.
* `401 Unauthorized`: Token ausente ou inválido.

---

### 3. Obter Fila de Serviços em Aberto (Restrito)

Lista solicitações recém-criadas na plataforma que ainda **não foram atribuídas** a nenhum técnico (Status: `Created`, `TechnicianId`: nulo).

* **Método:** `GET`
* **Rota:** `/api/ServiceRequest/newly`
* **Autenticação:** Requer Token JWT com privilégio Técnico (`Policy: Technician`)

**Respostas Esperadas**

* `200 OK`: Lista de requisições na fila de triagem.
* `403 Forbidden`: Acesso negado. Apenas técnicos podem acessar a fila geral.

---

### 4. Obter Detalhes da Solicitação

Retorna os dados completos de uma ordem de serviço. Possui proteção de dados (BOLA): Clientes só podem ler os dados se forem os proprietários; Técnicos podem ler dados gerais para execução.

* **Método:** `GET`
* **Rota:** `/api/ServiceRequest/{id}`
* **Autenticação:** Requer Token JWT (`Authorization: Bearer <token>`)

**Respostas Esperadas**

* `200 OK`: Retorna os dados completos, incluindo serviços atrelados (IDs), custos e técnico responsável.
* `404 Not Found`: Serviço não encontrado **ou** o serviço pertence a outro cliente (ocultação por segurança).

---

### 5. Atualizar Dados Técnicos / Orçamento (Restrito)

Permite que o técnico responsável insira ou altere o laudo técnico, o custo de peças e a lista de serviços executados. Este endpoint utiliza a técnica de **Snapshot/Reconciliação** para os `ServiceIds` (adiciona os novos, remove os ausentes e mantém os que já existiam).

* **Método:** `PUT`
* **Rota:** `/api/ServiceRequest/{id}`
* **Autenticação:** Requer Token JWT com privilégio Técnico (`Policy: Technician`)

**Corpo da Requisição (JSON)**

```json
{
  "technicalDiagnosis": "Constatada oxidação na placa e necessidade de troca de capacitor.",
  "partsCost": 150.00,
  "serviceIds": [1, 3]
}

```

**Respostas Esperadas**

* `200 OK`: Dados técnicos e lista de serviços atualizados com sucesso.
* `400 Bad Request`: Valores negativos ou array de serviços vazio.
* `403 Forbidden`: Usuário não é técnico.
* `404 Not Found`: Solicitação não encontrada.

---

### 6. Fluxo: Aceitar Solicitação (Restrito)

Técnico assume a responsabilidade por uma solicitação da fila. Altera o status para `InProgress` e vincula o `TechnicianId`.

* **Método:** `PUT`
* **Rota:** `/api/ServiceRequest/{id}/accept`
* **Autenticação:** Requer Token JWT com privilégio Técnico (`Policy: Technician`)

**Respostas Esperadas**

* `200 OK`: Solicitação aceita e status atualizado.

---

### 7. Fluxo: Aprovar Orçamento (Cliente)

O Cliente confirma a aprovação do orçamento elaborado pelo técnico. O sistema captura internamente o IP e o User-Agent do cliente para fins de auditoria (assinatura digital simplificada).

* **Método:** `PUT`
* **Rota:** `/api/ServiceRequest/{id}/approve-budget`
* **Autenticação:** Requer Token JWT (`Authorization: Bearer <token>`)

**Respostas Esperadas**

* `204 No Content`: Orçamento aprovado com sucesso.

---

### 8. Fluxo: Rejeitar Solicitação (Restrito)

Técnico recusa uma solicitação (ex: equipamento não suportado). Muda o status para `Rejected`.

* **Método:** `PUT`
* **Rota:** `/api/ServiceRequest/{id}/reject`
* **Autenticação:** Requer Token JWT com privilégio Técnico (`Policy: Technician`)

**Respostas Esperadas**

* `200 OK`: Solicitação rejeitada com sucesso.

---

### 9. Fluxo: Finalizar Serviço (Restrito)

Técnico conclui o trabalho executado no equipamento na bancada. Altera o status para `Finished`.

* **Método:** `PUT`
* **Rota:** `/api/ServiceRequest/{id}/finish`
* **Autenticação:** Requer Token JWT com privilégio Técnico (`Policy: Technician`)

**Respostas Esperadas**

* `200 OK`: Ordem de serviço finalizada.

---

### 10. Fluxo: Cancelar Serviço

Permite o cancelamento da ordem de serviço. Apenas possível se o trabalho ainda não tiver sido concluído.

* **Método:** `PUT`
* **Rota:** `/api/ServiceRequest/{id}/cancel`
* **Autenticação:** Requer Token JWT (`Authorization: Bearer <token>`)

**Respostas Esperadas**

* `200 OK`: Solicitação cancelada.

---

### 11. Obter Linha do Tempo (Timeline)

Recupera o histórico imutável de transições de status da solicitação. Ideal para exibir um rastreador visual estilo "correios" para o cliente.

* **Método:** `GET`
* **Rota:** `/api/ServiceRequest/timeline/{id}`
* **Autenticação:** Requer Token JWT (`Authorization: Bearer <token>`)

**Respostas Esperadas**

* `200 OK`: Retorna um array cronológico contendo os registros de histórico e data das movimentações.

---

### 12. Geração e Download de Documentos (PDF)

Gera em tempo real os documentos legais da plataforma para download direto ou visualização no navegador.

* **Baixar Orçamento (OS):**
* `GET` `/api/ServiceRequest/{id}/download-os`
* Gera o PDF com os dados técnicos e lista de serviços a executar.


* **Baixar Recibo de Quitação:**
* `GET` `/api/ServiceRequest/{id}/download-receipt`
* Gera o PDF comprovando o serviço finalizado e pago.



**Respostas Esperadas**

* `200 OK`: Retorna um arquivo binário do tipo `application/pdf`.

---

### 13. Notificações e Disparo de E-mails (Restrito)

Envia proativamente os documentos em PDF para o e-mail cadastrado do cliente. Esses endpoints delegam o trabalho para o serviço de infraestrutura (Resend).

* **Enviar Orçamento por E-mail:**
* `PUT` `/api/ServiceRequest/{id}/send-os`
* Utilizado após o técnico atualizar o orçamento (Endpoint 5).


* **Enviar Recibo de Quitação por E-mail:**
* `PUT` `/api/ServiceRequest/{id}/send-receipt`
* Utilizado após a finalização do pagamento.


    **Respostas Esperadas**

* `204 No Content`: E-mail gerado, PDF anexado e disparado com sucesso para a caixa de entrada do cliente. Requer credenciais de Técnico.* `404 Not Found`: Solicitação não encontrada ou não pertence ao usuário requerente.