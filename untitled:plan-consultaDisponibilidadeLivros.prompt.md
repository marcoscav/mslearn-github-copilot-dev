## Plan: Consulta de disponibilidade de livros

Completar a implementação parcial da consulta de disponibilidade no projeto `AccelerateDevGHCopilot`, mantendo o fluxo atual de console e o contrato do laboratório. A versão alvo já tem partes de `SearchBooks` em `ConsoleApp`, mas está inconsistente com `CommonActions` e com a injeção de `JsonData`; o plano corrige essa integração e valida o comportamento pelos dados JSON existentes.

**Passos**
1. Atualizar `CommonActions` com `SearchBooks = 64`, preservando os valores atuais das flags.
2. Corrigir `ConsoleApp` para receber `JsonData` no construtor e armazená-lo em `_jsonData`; manter o registro único `services.AddSingleton<JsonData>()` já presente em `Program.cs`.
3. Completar a integração em `PatronDetails`: incluir `SearchBooks` nas opções, aceitar `b` em `ReadInputOptions`, exibir a opção em `WriteInputOptions`, chamar `SearchBooks` e retornar a `PatronDetails` após a consulta.
4. Revisar `SearchBooks`: solicitar novamente enquanto o título estiver nulo ou em branco; chamar `EnsureDataLoaded`; localizar o livro sem diferenciar maiúsculas/minúsculas; informar título inexistente; localizar todos os exemplares pelo `BookId`; considerar o título disponível se algum exemplar não tiver empréstimo ativo (`ReturnDate == null`); caso todos estejam emprestados, exibir uma data de devolução de empréstimo ativo com a formatação esperada pelo roteiro.
5. Manter o escopo fora de empréstimos e reservas: não criar `Reservations.json`, repositórios, interfaces ou serviços novos.
6. Validar com `dotnet build`, `dotnet test` e execução manual do console. Como não há testes de console no projeto alvo, cobrir manualmente entrada vazia, título inexistente, livro disponível, livro emprestado, empréstimo devolvido e retorno ao menu.

**Relevant files**
- `/workspaces/mslearn-github-copilot-dev/LabFiles/03-develop-code-features/AccelerateDevGHCopilot/src/Library.Console/CommonActions.cs` — enum `CommonActions`; adicionar `SearchBooks`.
- `/workspaces/mslearn-github-copilot-dev/LabFiles/03-develop-code-features/AccelerateDevGHCopilot/src/Library.Console/ConsoleApp.cs` — corrigir a injeção e completar `ReadInputOptions`, `WriteInputOptions`, `PatronDetails` e `SearchBooks`.
- `/workspaces/mslearn-github-copilot-dev/LabFiles/03-develop-code-features/AccelerateDevGHCopilot/src/Library.Console/Program.cs` — somente confirmar o registro existente de `JsonData`.
- `/workspaces/mslearn-github-copilot-dev/LabFiles/03-develop-code-features/AccelerateDevGHCopilot/src/Library.Infrastructure/Data/JsonData.cs` — reutilizar `EnsureDataLoaded` e as listas carregadas; nenhuma alteração prevista.
- `/workspaces/mslearn-github-copilot-dev/LabFiles/03-develop-code-features/AccelerateDevGHCopilot/src/Library.ApplicationCore/Entities/Book.cs`, `BookItem.cs` e `Loan.cs` — relações `BookItem.BookId`, `Loan.BookItemId` e `Loan.ReturnDate` usadas na consulta.
- `/workspaces/mslearn-github-copilot-dev/Instructions/Labs/LAB_AK_03_develop_code_features.md` — requisitos funcionais e mensagens esperadas.
- `/workspaces/mslearn-github-copilot-dev/LabFiles/03-develop-code-features/AccelerateDevGHCopilot/tests/UnitTests` — não há testes de console; os testes existentes de serviços devem continuar passando.

**Verification**
1. Executar `dotnet build` em `/workspaces/mslearn-github-copilot-dev/LabFiles/03-develop-code-features/AccelerateDevGHCopilot` e confirmar ausência de erros, especialmente a resolução de `SearchBooks` e `_jsonData`.
2. Executar `dotnet test` no mesmo diretório e confirmar que os testes existentes continuam passando.
3. Executar `dotnet run --project src/Library.Console/Library.Console.csproj`, selecionar um patrono, usar `b` e consultar um título conhecido; verificar a mensagem de disponibilidade ou de empréstimo com data.
4. Repetir a execução com entrada vazia, título inexistente e título com diferenças de caixa; verificar a repetição da solicitação, a mensagem de inexistência e a busca case-insensitive.
5. Validar um empréstimo devolvido e, se os dados contiverem múltiplos exemplares, confirmar que um exemplar livre torna o título disponível; confirmar também o retorno a `PatronDetails` e o funcionamento de `q`.

**Decisions**
- A disponibilidade é por título e por todos os exemplares associados ao `BookId`, não somente pelo primeiro exemplar.
- Empréstimo ativo é definido por `ReturnDate == null`; a data de vencimento, inclusive vencida, não altera essa regra do requisito.
- Se houver múltiplos empréstimos ativos inconsistentes para o mesmo exemplar, selecionar deterministicamente o mais recente por `LoanDate` para a data exibida, sem alterar o modelo.
- O escopo limita-se à disponibilidade; empréstimos, reservas e persistência nova ficam para as etapas posteriores.
- Reutilizar `JsonData`; não criar novo repositório/serviço nem duplicar seu registro no DI.

**Further Considerations**
1. A implementação posterior de empréstimos pode alterar `Loans.json`; a consulta deve continuar usando o mesmo `JsonData` singleton e recarregar dados quando a camada de empréstimos fizer isso.
