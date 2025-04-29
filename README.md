
# Escopo Técnico do Projeto: Sistema de Planejamento de Produção

## Sobre o Projeto

Este projeto visa desenvolver um sistema robusto para gerenciar dados de um chão de fábrica de uma empresa metalúrgica e gerar planos de produção otimizados. O sistema será construído como uma API RESTful usando ASP.NET Web API, seguindo uma arquitetura baseada em Domain-Driven Design (DDD). O componente central de planejamento utilizará a biblioteca Google OR-Tools CP-SAT para modelar e resolver problemas complexos de agendamento, considerando as capacidades dos recursos, rotas de produção, estoque (surplus) e prazos de entrega.

**O que é o projeto?**

É um sistema de software que atua como o cérebro operacional de um ambiente de produção. Ele gerencia informações sobre máquinas, linhas, produtos, rotas e pedidos, e usa algoritmos avançados para determinar a melhor sequência e alocação de tarefas para atender à demanda e gera de forma automática um plano de produção otimizado para o planejador da empresa.

**Qual o intuito do projeto?**

O intuito principal é otimizar o processo de produção, garantindo a entrega no prazo dos pedidos, maximizando a utilização dos recursos e gerenciando eficientemente o estoque excedente (surplus), também visa maximizar o lucro de produção e apresentar possíveis custos por atraso. Ele busca transformar dados brutos do chão de fábrica em um plano de ação claro e executável.

**Explicação do processo:**

O projeto será instalado em uma empresa metalúrgica que fabrica ligas metálicas de diferentes tipos (cada liga possui seu proprio preço de venda e margem de lucro para empresa), a fábrica possui prédios produtivos, também chamado de linhas produtivas, onde cada uma tem sua própria configuração e capacidade. Cada linha produtiva é dividida em centros de trabalho (work center), onde cada work center produz um subproduto do produto final (ex. Aciaria, Laminação, Forjaria, etc.), cada work center é composto por diversas operações que são as fases para completar aquele subproduto (ex. Fusão a Quente, Resfriamento, Descarburação, etc.) cada operação tem sua própria capacidade produtiva (em toneladas por hora) e sua própria quantidade de postos de trabalho (ou máquinas), as operações são feitas de forma sequencial em sua grande maioria salvo algumas operações que tem mais de uma maquina (ex. a operação prensa tem 10 unidades, onde cada uma trabalha em paralelo). Cada produto planejado em uma linha tem sua própria rota de operação a qual ele deve ser submetido, o produto planejado em uma linha deve ser mantido naquela linha até o fim. Um produto novo pode entrar na mesma linha onde existe um produto corrente, se e somente se, o produto novo entrar em algum work center anterior ao produto corrente estiver e se não houver nenhum outro planejamento do produto corrente naqueles work centers, isso é válido pois a produção de produtos novos envolve setup das operações (onde cada produto tem seu próprio tempo de setup) e isso gera um tempo não produtivo, então quanto menos setup houver, melhor para operação. Também por isso, cada work center tem uma capacidade produtiva ideal, ou seja, um número em toneladas que indica que se produzir menos é prejuízo e se produzir mais pode gerar falhas, isso implica na produção por _runs_ (corridas), isto é, a produção em cada work center é feita por lotes produtivos, **exemplo**: Se o _"Produto A"_ vai produzir 60 toneladas na _"Aciaria"_ e esta por sua vez tem uma capacidade produtiva ideal de 40 toneladas, isso gerará: 1ª run = 40 toneladas (em _x_ horas) + 2ª run = 40 toneladas (em mais _x_ horas), resultando em 80 toneladas (60 toneladas para entregar para o próximo work center + 20 toneladas de estoque do _"Produto A"_ na _"Aciaria"_).
O planejador da empresa recebe uma lista de produtos a serem produzidos, a quantidade em tonelada de cada produto e uma data limite para esta produção (deadline), ele organiza de forma lógica onde cada produto será alocado (linhas), a sequencia em que os produtos serão fabricados (Produto "A" depois Produto "B") e o tempo total de produção, levando em consideração a configuração atual das operações, o tempo de cada uma, o tempo de setup e o tempo de transporte do produto entre work centers. Como resultado ele entrega uma tabela que representa uma _timeline_ indicando  data de inicio e fim da operação de cada produto e de cada work center, nessa tabela, caso exista algum produto que ficará de fora do planejamento por quebra de deadline, ele é indicado como _"não planejado"_. Junto a esta tabela ele envia um report indicando o lucro total do planejamento, estoque gerado (teórico) por produto e work center, a data final real de conclusão e caso exista produtos que ficaram de fora do planejamento ele discrimina a lista de produto.

## Arquitetura

O projeto segue uma arquitetura em camadas baseada em DDD para separar preocupações e promover um design limpo e manutenível:

-   **Domain:** O núcleo, contendo a lógica de negócio, entidades, objetos de valor, agregações e interfaces (repositórios, serviços de domínio).
    
-   **Application:** Orquestra as operações de domínio para executar casos de uso da aplicação. Inclui DTOs e serviços de aplicação (como o serviço de planejamento).
    
-   **Infrastructure:** Implementa detalhes técnicos como acesso a dados (usando Entity Framework Core), mocks para testes, etc.
    
-   **Presentation:** A camada de API (ASP.NET Web API) que expõe a funcionalidade do sistema.
    

## Modelo de Dados (Domain.Entities)

O modelo de dados representa as entidades do chão de fábrica e da demanda de produção. As classes C# herdam de uma `BaseEntity` comum e utilizam `Data Annotations` para mapeamento com Entity Framework Core.

-   **BaseEntity:** Classe base abstrata com `ID` (chave primária), `Enabled` (para soft delete), `CreatedAt`, e `LastUpdate`.
    
-   **OperationType:** Tipos de processos (Fusão, Aquecimento).
    
-   **Operation:** Máquinas ou processos específicos em um Work Center. Inclui `SetupTimeInMinutes` e `Capacity` (Ton/Hora).
    
-   **WorkCenter:** Agrupamentos de máquinas/operações (Aciaria, Laminação). Inclui `OptimalBatch` (Ton) para divisão de runs.
    
-   **Line:** Linhas de produção físicas (Prédio 01).
    
-   **WorkCenterOperationRoute:** Rota de tipos de operação dentro de um Work Center. Inclui `Order`, `Version`, datas de efetividade e `TransportTimeInMinutes` (entre etapas dentro do WC).
    
-   **LineWorkCenterRoute:** Rota de Work Centers dentro de uma Linha. Inclui `Order`, `Version`, datas de efetividade e `TransportTimeInMinutes` (entre WCs na Linha).
    
-   **ProductAvailablePerLine:** Associa Produtos a Linhas onde podem ser produzidos.
    
-   **Product:** Produtos a serem fabricados. Inclui `Name`, `UnitPricePerTon`, `ProfitMargin`, `Priority`, `PenalityCost`.
    
-   **ProductOperationRoute:** Rota de tipos de operação necessária para produzir um Produto. Inclui `Order`, `Version`, datas de efetividade.
    
-   **SurplusPerProductAndWorkCenter:** Estoque de surplus (Ton) de um Produto em um Work Center. Inclui `Surplus` (Ton) e `LastUpdate` (do estoque).
    
-   **ProductionOrder:** Pedido de produção. Inclui `OrderNumber`, `EarliestStartDate`, `Deadline`.
    
-   **ProductionOrderItem:** Item em um pedido, especificando Produto e `Quantity` (Ton).
    

## Processo Base e Restrições

O processo de produção e as regras que o governam são fundamentais para a modelagem no OR-Tools.

-   **Fluxo Unilinha:** Um produto, uma vez iniciado, permanece na mesma linha até a conclusão.
    
-   **Sequência Rígida (WC e Operação):** Produtos movem-se sequencialmente através dos Work Centers em uma linha e através das operações dentro de um Work Center, conforme definido pelas rotas. Uma etapa deve terminar para a próxima começar para o mesmo produto.
    
-   **Paralelismo em Operações:** Se um Work Center possui múltiplas máquinas (`Operation`s) do mesmo `OperationType`, runs de produtos podem ser processados simultaneamente nessas máquinas paralelas.
    
-   **Divisão em Runs e Surplus:** A quantidade necessária de um `ProductionOrderItem` é dividida em "runs" com base no `OptimalBatch` do primeiro Work Center da rota do produto. Qualquer quantidade produzida além da demanda líquida (após deduzir surplus inicial) torna-se surplus gerado. O surplus inicial no primeiro Work Center é deduzido da quantidade a ser agendada.
    

### Restrições Críticas (Modelagem no OR-Tools)

As seguintes restrições são rígidas (devem ser cumpridas) e requerem modelagem cuidadosa no solver:

-   **R5: Bloqueio de Work Center:** Um Work Center fica bloqueado para a entrada de um _novo_ produto (início da primeira operação desse produto no WC) se _qualquer_ máquina (`Operation`) dentro dele estiver ocupada por _qualquer outro_ produto. Produtos esperando por um WC formam uma fila.
    
-   **R6: Regra de Fluxo na Linha ("No-Passing"):** Se um produto A iniciou na linha antes de um produto B, então o produto B não pode iniciar em _qualquer_ Work Center igual ou posterior a A. Esta regra se aplica universalmente.
    
-   **R7: Tempo de Setup Agregado Condicional:** Em uma `Operation` (máquina), há um `SetupTimeInMinutes` em caso de troca de produto. Em um grupo de máquinas paralelas (mesmo `OperationType` em um `WorkCenter`), o tempo de setup para a _primeira_ vez que um produto passa por _qualquer_ uma dessas máquinas é o _máximo_ `SetupTimeInMinutes` entre elas. Este setup agregado atrasa o início do primeiro run do produto nesse grupo. (O setup para runs subsequentes do mesmo produto no grupo paralelo requer refinamento).
    
-   **R8: Tempos de Transporte:** Há tempos de transporte definidos: após concluir uma operação para a próxima dentro do mesmo Work Center (`WorkCentersOperationRoutes.TransportTimeInMinutes`) e após concluir um Work Center para o próximo na linha (`LinesWorkCentersRoutes.TransportTimeInMinutes`). Estes tempos devem ser respeitados como atrasos entre as etapas agendadas.
    
-   **Deadlines:** Cada `ProductionOrderItem` deve ser concluído até o seu `Deadline`.
    

## Serviço de Planejamento (Application.Services.ProductionSolverService)

O `ProductionSolverService` é o componente que executa o planejamento.

-   **Tecnologia:** Google OR-Tools CP-SAT Solver.
    
-   **Entrada:** Obtém dados de configuração, estoque e ordens de produção através de interfaces de serviços de domínio injetadas (seguindo o padrão Repositório -> Serviço de Domínio).
    
-   **Processo:**
    
    1.  Busca os dados necessários (entidades habilitadas, ordens pendentes).
        
    2.  Calcula a quantidade líquida a agendar para cada item de ordem (deduzindo surplus inicial no primeiro WC).
        
    3.  Determina o número de runs necessários para cada item/quantidade líquida com base no `OptimalBatch` do primeiro WC.
        
    4.  Cria variáveis do solver (`BoolVar`, `IntVar`, etc) para cada run de cada etapa da rota do produto, em cada recurso candidato (linha, WC, operação/máquina).
        
    5.  Adiciona restrições ao modelo CP-SAT para refletir as regras de processo (Alocação, No Overlap, Precedências, Tempos de Transporte, Deadlines, Bloqueio de WC, No-Passing, Setup Agregado).
        
    6.  Define a função objetivo.
        
    7.  Invoca o solver.
        
    8.  Processa a solução do solver para preencher o DTO de saída.
        
-   **Lógica de Modelagem:** Traduz as regras de negócio em restrições matemáticas para o solver, utilizando variáveis de intervalo, variáveis booleanas e restrições lógicas (`AddMaxEquality`, `AddMinEquality`, `AddNoOverlap`, `WithTransitionTimes`, `AddExactlyOne`, `AddBoolOr`, `OnlyEnforceIf`).
    

## Objetivos do Projeto

O objetivo principal do planejamento é a **Entrega no Prazo (On-Time Delivery)**.

-   **Objetivo do Solver:** Maximizar o número de itens de ordem de produção agendados dentro de seus respectivos `Deadline`s.
    
-   **Priorização:** Itens de produtos com maior `Priority` (menor valor numérico) são priorizados na função objetivo do solver, recebendo um peso maior para incentivar o solver a agendá-los primeiro, especialmente quando há conflitos de recursos.
    
-   **Saída:** O serviço de planejamento retorna um DTO (`ProductionPlanResultDto`) que detalha o plano gerado.
    

## Saída do Planejamento (Application.DTOs.ProductionPlanResultDto)

O resultado do planejamento é encapsulado em um DTO para ser retornado pela API.

-   **ProductionPlanResultDto:**
    
    -   `PlanOverallDeadline` (`DateTime`): O deadline geral considerado para o plano.
        
    -   `PlanActualCompletionDate` (`DateTime`): Data/hora de conclusão da última tarefa agendada.
        
    -   `ScheduledRuns` (`List<ScheduledRunDetailDto>`): Detalhes de cada corrida/run agendada na timeline.
        
    -   `GeneratedSurplus` (`List<SurplusSummaryDto>`): Resumo do surplus gerado por produto e Work Center.
        
    -   `EstimatedTotalProfit` (`decimal`): Lucro total estimado (lucro dos agendados - penalidades dos não agendados).
        
    -   `UnscheduledOrderItems` (`List<UnscheduledOrderItemDto>`): Itens de ordem que não puderam ser agendados dentro das restrições.
        
    -   `SolverStatus` (`string`): Status retornado pelo solver (e.g., OPTIMAL, FEASIBLE, INFEASIBLE).
        
    -   `SolverSolveTime` (`TimeSpan`): Tempo que o solver levou para resolver.
        
-   **ScheduledRunDetailDto:** Detalhes de um run agendado (Início/Fim, Produto, Run#, WC, Linha, Operação, Quantidade Ton).
    
-   **SurplusSummaryDto:** Resumo do surplus gerado (Produto, WC, Quantidade Ton).
    
-   **UnscheduledOrderItemDto:** Detalhes de um item não agendado (Ordem, Item, Produto, Quantidade Ton necessária).