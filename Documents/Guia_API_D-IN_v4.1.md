# Guia de implementação da API D-IN

------------------------------------------------------------------------

## Página 1

1.  Guia de implementação da API D-IN Versão 4.1 Data Revisão
    19/04/2021  1. Introdução 1.1. O que é o programa D-IN? 1.2. Qual o
    Benefício do Programa? 1.3. O que há nesse guia?
2.  Fases de implementação 2.1. Reunião de Alinhamento 2.2. Homologação
    Técnica (API) 2.3.Homologação de Dados 2.4. Imersão na
    Concessionaria 3 Referencial Técnico 3.1. Regras de Negócio 3.2
    Implementação da API 3.2.1. Ambientes 3.2.2. Autenticação 3.2.3.
    Contrato de dados 3.2.4. Exemplo JSON
3.  Preparação e Testes
4.  Códigos de Retorno
5.  Detalhamento de Response Code 400 ou 413
6.  Departamentos
7.  Planos de Contas
8.  Histórico das versões 1. Introdução 1.1. O que é o programa D-IN?
    Também conhecido como DEF, trata-se de uma plataforma de análise de
    dados com informações relevantes sobre a situação financeira,
    performance e produtividade do concessionário em tempo estático.
    Permite que o concessionário envie uma visão consolidada de sua
    saúde financeira com a ótica de frentes de negócio (Produtos, Peças,
    Usados, Serviços e Outras Linhas) através da API para integração em
    seu DMS. Como resultado, recebe uma análise de sua performance
    representada por KPIs, possibilitando a execução de planos de ação
    para aprimorar seus resultados. 1.2. Qual o Benefício do Programa? O
    Programa gera informações padronizadas da Rede proporcionando maior
    gestão do negócio, identificação de processos críticos e ações
    estratégicas e táticas que possam melhorar a rentabilidade dos
    concessionários. Também propicia um ambiente de aprendizados e
    experiências de boas práticas. 1.3. O que há nesse guia? Esta página
    contém informações para implementação e manutenção da API do
    programa AGCO D-IN. Os tópicos estão separados de acordo com algumas
    características técnicas e de negócio. Também está disponível as
    etapas para implementação, desde os testes até a entrada em produção
    junto à concessionária. O documento com as informações da API estão
    disponíveis aqui:
    https://api-qa.sa.agcocorp.com/swagger/?urls.primaryName=D-IN#/dfs-controller
    /saveUsingPOST
9.  Fases de implementação A Implementação do programa D-In ocorre em 4
    etapas macro:

------------------------------------------------------------------------

## Página 2

1.  
2.  
3.  
4.  Reunião de Alinhamento Homologação Técnica Homologação de Dados
    Imersão com a Concessionária 2.1. Reunião de Alinhamento Esse é o
    momento em que os se reúnem para o alinhamento inicial do
    programa.stakeholders A participação do time da Concessionária
    (composto por Gestão, Contábil e TI), do DMS e da AGCO
    (Desenvolvimento de Concessionária, Finanças e TI) é obrigatória.
    Aqui o programa é revisitado e alguns pontos são compartilhados para
    esclarecer as dúvidas iniciais, tais como objetivo do programa,
    formas de comunicação, segregação de ambientes, sistema onde a
    gestão contábil é realizada, plano de contas entre outros. Define-se
    também as datas para a próxima etapa: a .homologação técnica Novos
    encontros para alinhamentos pré-homologação técnica não estão
    descartados e isso deve partir do time da Concessionária ou DMS, se
    assim acharem necessário. 2.2. Homologação Técnica (API) A AGCO atua
    com ambientes segregados de Desenvolvimento (DV), Homologação (QA) e
    Produção (PD). Todo o processo de homologação técnica é realizado em
    nosso ambiente de QA. A homologação requer evidências das
    implementações presentes no item " ". Desta forma, durante a essa
    etapa é obrigatório Regras de Negócio que o DMS esteja preparado
    para simular erros, alterar dados, bloquear portas de acesso e assim
    por diante para que as evidências de sucesso sejam coletadas. Ao
    término da homologação técnica da API pela TI da AGCO, será
    comunicado formalmente que a API está apta a receber os dados do DMS
    e seguir para a próxima etapa: homologação de dados. Com isso, é
    possível iniciar a homologação dos dados com o time Financeiro da
    AGCO. 2.3.Homologação de Dados Nessa etapa, a área de negócios da
    AGCO (Financeiro) realiza a validação dos dados recebidos via API,
    confrontando com o balancete contábil enviado pelo DMS para o mesmo
    período (Mês/Ano). Embora relativamente simples, é nessa fase em que
    se identificam as maiores inconsistências de dados e,
    consequentemente, onde acaba se demandando mais tempo. Muitos dos
    pontos identificados durante a Homologação de Dados podem ser
    evitados se todas as regras de negócio forem seguidas. Assim,
    sugere-se que elas sejam utilizadas como principal referência e, em
    caso de qualquer dúvida, acione o time do D-In. Com ambas
    homologações concluídas (API e Dados), o próximo passo é a .imersão
    na concessionária 2.4. Imersão na Concessionaria O processo de
    imersão na concessionária deve ser previamente alinhado entre AGCO,
    concessionário e o time do DMS, já que é durante essa etapa que é
    feita a virada para o ambiente de Produção do D-In. Durante a
    imersão, que costuma levar aproximadamente 2 dias, um time técnico
    atua localmente com o Concessionário, auxiliando no entendimento do
    programa, fluo de informações, acesso aos dados de retorno e assim
    por diante. Nesse momento, os dados produtivos da concessionária
    também são validados e ajustes finos de configurações do contas e"De
    x Para", por exemplo, são realizados. Desta forma, como haverá uma
    alteração do apontamento de ambiente, recomenda-se que disponibilize
    ao Concessionário a opção de alterar o destino do envio dos dados,
    entre QA e PD, de maneira autônoma e simples para o usuário final.
    Finalizada essa fase, o Concessionário estará apto a utilizar o
    programa D-In regularmente. 3 Referencial Técnico 3.1. Regras de
    Negócio Comunicação online através de uma interface API
    disponibilizada pela AGCO. O CNPJ Matriz utilizado como origem deve,
    obrigatoriamente, ser o cadastrado junto à AGCO.

------------------------------------------------------------------------

## Página 3

Eventualmente, existem casos em que o CNPJ Matriz do grupo (0001-XX) não
é uma operação vinculada à montadora. Adote uma forma de contornar esses
casos. Como opção, avalie a possibilidade do concessionário ter a opção
de escolher ou definir CNPJ Matriz utilizado para envio. Envio de todos
os CNPJs ativos cadastrados na base de dados da AGCO que tenham relação
com a Matriz do Grupo (CNPJ do contrato de concessão) é
obrigatório. Impossibilite, via código, o envio individual.  Implemente
todos os Response Codes previstos e tratamento das mensagens com LOGs
e/ou mensagem amigável ao usuário. Garanta que todas as contas que
possuem movimentação e saldo tenham a relação "De/Para" e que todas
sejam transmitidas. Impeça a tentativa de envio de contas sem a relação
"De/Para", apresentando um erro amigável que facilite a identificação do
erro na origem. Para valores que representam saldo em mais de um
departamento, o mesmo deve ser enviado por todos. É permitido o envio da
mesma conta pra vários departamentos. Caso a operação possua apenas uma
unidade, ou para envio dos dados da Matriz, considere o CNPJ do = na
API. HeadOffice BranchCode A soma das contas de referencial D-IN
(Iniciando de 1 à 8 . Ex.: 1111 até 8112 ) referentes a contas do Ativo
(+/-) / Passivo  (+/-) e DRE (+/-) tem que ser igual a 0 (zero). Esse é
o valor enviado no campo  na API.totalValue Caso as informações de plano
de contas das lojas já tenham sido enviadas anteriormente (validação
ocorre pela PK), o conteúdo é considerado como retificação. 3.2
Implementação da API 3.2.1. Ambientes A AGCO atua com ambientes
segregados de Desenvolvimento (DV), Homologação (QA) e Produção (PD).
Para esse programa, contudo, a AGCO adotou o uso apenas dos ambientes de
QA e PD.  A seguir, encontram-se as URLs dos respectivos ambiente: URL
em : ambiente de QA https://apps-new-qa.agcoonline.com.br/dfs/save URL
em : ambiente de PD https://app.agcoonline.com.br/dfs/save 3.2.2.
Autenticação Para o programa D-In não há autenticação para uso das URLs.
3.2.3. Contrato de dados Campo Tipo Obrigatório Descrição balanceValue
Decimal Sim Saldo do plano de contas dealerHeadOfficeCode String Sim
CNPJ da matriz dfsPk - codeDFSPlan String Sim Código do plano de conta
dfsPk - dealerBranchCode String Sim CNPJ da filial dfsPk - periodMonth
String Sim Mês do período contábil dfsPk - periodYear String Sim Ano do
período contábil dfsPk - sectionCode String Sim Código do departamento
3.2.4. Exemplo JSON Exemplo JSON de request: \[ { "balanceValue": 0,
"dealerHeadOfficeCode": "12.345.678/0009-10", "dfsPk": { "codeDFSPlan":
"1229", "dealerBranchCode": "98.765.432/0001-10", "periodMonth": "06",
"periodYear": "2020", "sectionCode": "1401" } }\] Exemplo JSON de
response:

------------------------------------------------------------------------

## Página 4

1.  
2.  
3.  
4.  
5.  { "dealerBranchCodes": \[ "98.765.432/0001-10" \],
    "dealerHeadOfficeCode": "12.345.678/0009-10", "periodMonth": "06",
    "periodYear": "2020", "protocol":
    "ff80808173714dd50173715102850000", "totalValue": 0 }
6.  Preparação e Testes Antes do início dos testes de Dados: DMS envia
    balancete contábil de um determinado período extraído da base de
    dados contábil do dealer em formato padrão do sistema contábil da
    concessionária.  DMS envia o mesmo pacote de dados do item 1 (Mesmo
    período, contas e saldos) via API. Essas mesmas informações
    incluídas na planilha deverão ser enviadas pela API de integração,
    pois essa planilha servirá para validar dados em relação ao
    informado na API. Este balancete tem que conter todas as contas do
    Ativo, Passivo e DRE, onde  a soma de todas as contas deverão
    totalizar 0 (zero). AGCO validará as informações enviadas pela API
    com a planilha de dados contábeis. Durante os testes, serão
    verificados os cenários abaixo: Envio de apenas um CNPJ (Filial ou
    Matriz). Envio da mesma conta pra vários departamentos. Soma das
    contas de referencial D-IN diferente de zero. Validar evidencias dos
    Response Codes relacionados nesse documento. Implementação e
    tratamento dos Response Codes. Novos poderão e serão solicitados ao
    decorrer do processo para garantir a sustentabilidade da solução.
7.  Códigos de Retorno Response Code  Status / Mensagem de retorno
    Descrição 200 OK A requisição enviada foi bem sucedida. 201 Criado A
    requisição enviada foi bem sucedida e um novo recurso foi criado
    como resultado. 400 Solicitação incorretaA requisição não foi
    atendida pois foi enviada com sintaxe errada. Lista de erros de
    sintaxe: O campo balanceValue não pode ser nulo O campo
    dealerHeadOfficeCode não pode ser nulo O campo codeDFSPlan não pode
    ser nulo O campo dealerBranchCodenão pode ser nulo O campo
    periodMonth não pode ser nulo O campo periodMonth precisa estar
    entre 1a 12 O campo periodYear não pode ser nulo O campo periodYear
    precisa ter 4 dígitos O campo sectionCode não pode ser nulo As
    seguintes filiais não foram entradas no D-IN enviado: Dados não
    recebidos pela AGCO devido valores divergentes. A soma das contas de
    Ativo, Passivo e DRE devem ser igual a zero. (Saldo atual: xx,xx)
    401 Não autorizado A requisição não foi atendida pois houve um erro
    de autenticação e o cliente deve autenticar para obter a resposta
    solicitada (não aplicával ao endpoint de save) 403 Proibido A
    requisição não foi atendida pois o cliente não possui direitos de
    acesso ao conteúdo. Diferente do retorno 401, nesse caso a
    identidade do cliente é conhecida (não aplicável ao endpoint de
    save) 404 Não encontrado A requisição não foi atendida pois o
    servidor não encontrou algum recurso solicitado.

------------------------------------------------------------------------

## Página 5

413 Tamanho máximo do corpo limitado a 4MB A requisição não foi atendida
pois é maior que o limite definido pelo servidor. 6. Detalhamento de
Response Code 400 ou 413 Caso os critérios não sejam atendidos, será
retornado um erro .400 ou 413 Neste caso, os dados incorretos devem ser
corrigidos antes de uma nova tentativa de envio. Regra Descrição
Mensagem de erro Exemplo de valor enviado 1 O campo dealerHeadOfficeCode
não pode ser nulo Status: "400" -- "O campo dealerHeadOfficeCode não
pode ser nulo""12.345.678 /0009-10" 2 O campo balanceValue não pode ser
nulo Status: "400" -- "O campo balanceValue não pode ser nulo" 3 3 O
campo sectionCode não pode ser nulo Status: "400" -- "O campo
sectionCode não pode ser nulo" "1401" 4 O campo sectionCode não pode ser
inválido Status: "400" -- "SectionCode 
inválida"`<valor recebido>`{=html}  "1401" 5 O campo periodMonth não
pode ser nulo Status: "400" -- "O campo periodMonth não pode ser nulo"
"06" 6 O campo periodMonth tem que ser um mês válido entre 1 a 12
Status: "400" -- "O campo periodMonth precisa estar entre 1 a 12""06" 7
O campo periodYear não pode ser nulo Status: "400" -- "O campo
periodYear não pode ser nulo" "2021" 8 O campo periodYear tem que ser um
ano válido com 4 digitos Status: "400" -- "O campo periodYear precisa
ter 4 dígitos""2021" 9 O campo dealerBranchCode não pode ser nulo
Status: "400" -- "O campo dealerBranchCode não pode ser nulo""98.765.432
/0001-10" 10 O campo codeDFSPlan não pode ser nulo Status: "400" -- "O
campo codeDFSPlan não pode ser nulo" "1229" 11 O campo codeDFSPlan não
pode ser nulo Status: "400" -- "DFS Plan
inválido"`<valor recebido>`{=html}  "1229" 12 Deverá vir informações de
todas as filiais da Matriz Status: "400" -- "As seguintes filiais não
foram encontradas no D-IN enviado: " - 13 Um PK (codeDFSPlan,
dealerBranchCode, periodYear, periodYear, sectionCode) não pode
ser enviado mais de uma vez no mesmo envio Status: "400" -- "O DMS não
pode enviar o mesmo PK mais de uma vez (codeDFSPlan, dealerBranchCode,
periodYear, periodYear, sectionCode) no mesmo envio" - 14 É permitido
apenas um período no envio (periodYear, periodMonth)Status: "400" --
"Foram encontrados períodos distintos no arquivo, favor enviar apenas um
período por arquivo" - 15 A soma das contas de referencial D-IN
(Iniciando de 1 à 8 . Ex.: 1111 até 8112 ) referentes a contas do Ativo
(+/-) / Passivo  (+/-) e DRE (+/- ) tem que ser igual a 0 (zero).
Status: "400" -- "Dados não recebidos pela AGCO devido valores
divergentes. A soma das contas de Ativo, Passivo e DRE devem ser igual a
zero. (Saldo atual: xx,xx)" - 16 O tamanho do arquivo não pode exceder
mais do que 4mb Status: "413" -- "Max body size limited to 4MB" - 7.
Departamentos Segue abaixo os departamentos utilizados pela AGCO: Código
Descrição 1101 Maquinas Novas 1102 Maquinas Usadas 1201 Peças 1301
Serviços 1401 Administrativo 1501 Outros Negócios 8. Planos de Contas
Segue abaixo planos de contas utilizados pela AGCO:

------------------------------------------------------------------------

## Página 6

Código Descrição 1111 Disponibilidades Imediatas 1121 Contas a Receber
Maquinas Novas 1122 Contas a Receber Maquinas Usadas 1123 Contas a
Receber Peças 1124 Contas a Receber Serviços 1125 Contas a Receber
Comissões e Consórcios 1126 Contas a Receber Outros 1127 Empresas
Coligadas 1128 Consorcio em Andamento 1131 Adiantamentos Fornecedores
1132 Adiantamentos Funcionários 1133 Despesas Antecipadas - Curto Prazo
1139 Outros Adiantamentos 1141 Impostos a Compensar e Recuperar Curto
Prazo 1151 Máquinas e Implementos AGCO - Novos 1152 Máquinas e
Implementos AGCO - Usados 1153 Máquinas e Implementos Outras Marcas -
Novos 1154 Máquinas e Implementos Outras Marcas - Usados 1155 Peças AGCO
1156 Peças Outras Marcas 1157 Estoque de Outros Negócios - AGCO 1158
Estoque de Outros Negócios 1159 (-) Estoques - Ajuste a Valor Presente
1160 (-) Provisão de Estoque Obsoleto 1194 (-)Devolução de Vendas 1195
Valores em Transito 1196 (-) Contas a Receber - Ajuste a Valor Presente
1197 (-) PCLD - Provisão de Créditos Duvidosos 1198 Outros Créditos 1199
Fundo AGCO 1211 Contas a Receber - Vendas - Longo Prazo - Novos 1212
Contas a Receber - Vendas - Longo Prazo - Usados 1213 Aplicações
Financeiras - Longo Prazo 1214 Consórcio 1215 Créditos com Partes
Relacionadas - LP 1216 (-) Investimentos Permanentes - Ajuste a Valor
Presente 1217 (-) Contas a Receber LP - Ajuste a Valor Presente 1221
Impostos Diferidos 1225 Despesas Antecipadas - Longo Prazo 1229 Outros
Créditos

------------------------------------------------------------------------

## Página 7

1231 Terrenos 1232 Construções e Prédios 1233 Máquinas/equipamentos e
ferramentas 1234 Móveis e Utensílios 1235 Veículos 1236 Frota para
Locação 1237 Imobilizado outros 1238 Benfeitorias em prédio de Terceiros
1239 Consorcios em Andamentos para Imobilziado 1240 Computadores e
Perifericos 1296 (-) Aplicações Financeiras LP - Ajuste a Valor Presente
1297 (-) Ativo Imobilizado - Ajuste a Valor Presente 1298 Intangível
1299 (-) Depreciação e Amortização 2111 Fornecedores Maquinas Novas 2112
Fornecedores Maquinas Usadas 2113 Fornecedores Peças 2114 Fornecedores
Serviços 2115 Fornecedores Outros 2116 Contas a Pagar Partes
Relacionadas 2117 Outras contas a pagar 2118 Contas a Pagar Empresas
Coligadas 2121 Financiamentos AGCO FINANCE CP 2122 Empréstimos Bancários
CP 2123 Outros Empréstimos CP 2124 Financiamentos Outros Bancos CP 2125
Empréstimos de Sócios CP 2126 Leasing a Pagar CP 2127 Duplicatas
Descontadas 2128 Financiamentos FINAME 2130 (-) Ajuste a Valor
Presente - Empréstimos 2131 Salários e Bônus a Pagar 2132 Obrigações
Trabalhistas a Recolher 2133 Provisões Trabalhistas 2141 Obrigações
Tributárias a Recolher 2142 Outras Obrigações Fiscais a Recolher 2151
Adiantamento de Clientes 2152 Outros Adiantamentos 2161 Dividendos a
Pagar 2162 Consórcios a Pagar 2163 Faturamento para Entrega Futura

------------------------------------------------------------------------

## Página 8

2164 Receitas e Custos Diferidos 2165 Impostos Diferidos 2166 Valores em
Transito 2197 (-) Devoluções de Compras 2198 (-) Contas a Pagar -
Reclassificação para Longo Prazo 2199 (-) Ajuste a Valor Presente -
Contas a Pagar 2211 Financiamentos AGCO FINANCE LP 2212 Financiamentos
Outros Bancos LP 2213 Empréstimos Bancários LP 2214 (-) Ajuste a Valor
Presente - Empréstimos 2221 Contas a Pagar - Empresas Coligadas LP 2222
Contas a Pagar e Fornecedores LP 2223 Outros Investimentos a pagar 2224
Contas a Pagar de Partes Relacionadas LP 2225 Outros Empréstimos LP 2226
Parcelamento de Tributos 2227 Provisões para Contingências 2231 Capital
Social Integralizado 2232 Adiantamentos para Futuro Aumento de Capital
2233 Reservas de Lucro 2234 Reservas de Capital 2235 (-) Ações/Quotas em
Tesouraria 2236 (-) Dividendos Distribuídos 2237 (-) Juros Capital
Próprio 2238 (-) Prejuízos Acumulado 2239 Resultado do Exercício 2299
(-) Ajuste a Valor Presente - Outras Contas a Pagar LP 3111 Comissões
sobre Vendas - Consórcio 3112 (-) Dedução de Comissões sobre Vendas -
Consórcio 3113 Comissões sobre Vendas de Máquinas 3114 (-) Dedução de
Comissões sobre Vendas de Máquinas 3115 Receita com Outras Comissões
3116 (-) Dedução de Outras Comissões 3117 Máquinas AGCO - Novos 3118 (-)
Dedução de Máquinas AGCO - Novos 3119 Implementos AGCO - Novos 3120 (-)
Dedução de Implementos AGCO - Novos 3121 Implementos Outras Marcas -
Novos 3122 (-) Dedução de Máquinas Outras Marcas - Novos 3131 Máquinas e
Implementos AGCO - Usados 3132 (-) Dedução de Máquinas e Implementos
AGCO - Usados

------------------------------------------------------------------------

## Página 9

3133 Máquinas e implementos Outras Marcas - Usados 3134 (-) Dedução de
Máquinas e Implementos Outras Marcas - Usados 3211 Receita com Comissões
Sobre Vendas de Peças 3212 (-) Dedução de Comissões Sobre Vendas de
Peças 3213 Receitas Peças AGCO 3214 Receitas Peças Oficina AGCO 3215 (-)
Dedução de Peças AGCO 3216 (-) Dedução de Peças Oficina AGCO 3221 Peças
Outras Marcas 3222 Peças Oficina Outras Marcas 3223 (-) Dedução de Peças
Outras Marcas 3224 (-) Dedução de Peças Oficina Outras Marcas 3225
Lubrificantes 3226 (-) Dedução de Lubrificantes 3311 Receita com
Serviços Prestados a Clientes 3312 (-) Dedução de Serviços Prestados a
Clientes 3313 Receita com Serviços Internos 3314 (-) Dedução de Serviços
Internos 3315 Receita com Serviços de Garantia 3316 (-) Dedução de
Serviços de Garantia 3317 Receita com Serviços Recall 3318 (-) Dedução
de Serviços Recall 3319 Receita com KM de Serviços de Assistência a
Campo 3320 (-) Dedução de KM de Serviços de Assistência a Campo 3321
Receita com deslocamento de Assistência a Campo 3322 (-) Dedução de
deslocamento de Assistência a Campo 3323 Serviços Realizados por
Terceiros 3324 (-) Dedução de Serviços Realizados por Terceiros 3411
Receitas de Tecnologia - ACGO 3412 Receita Outros Negócios AGCO 3413 (-)
Dedução de Receitas Tecnologia - AGCO 3414 (-) Dedução de Receitas com
Outros Negócios - AGCO 3511 Receita Outros Negócios 3512 (-) Dedução de
Receitas com Outros Negócios 4117 Custo Máquinas AGCO - Novos 4119 Custo
Implementos AGCO - Novos 4121 Custo Máquinas Outras Marcas - Novos 4131
Custo Máquinas e Implementos AGCO - Usados 4133 Custo Máquinas e
implementos Outras Marcas - Usados 4213 Custo Peças AGCO 4214 Custo
Peças Oficina AGCO

------------------------------------------------------------------------

## Página 10

4221 Peças Outras Marcas 4222 Peças Oficina Outras Marcas 4311 Materiais
de Consumo - Oficina 4312 Salários e Encargos dos Técnicos 4313 Custos
Serviços Realizados por Terceiros 4314 Custo com deslocamento técnicos
4315 Custos com Pós - Venda/Garantias Indeferidas e Retrabalhos 4319 Uso
e Consumo Oficina 4320 Ferramental de pequeno valor 4399 Outros Custos
Sobre Serviços 4411 Custo de Tecnologia ACGO 4412 Custo com Outros
Negócios - AGCO 4511 Custo com Outros Negócios 5111 Brindes 5112
Comissões e Encargo Folha de Pagamento 5113 Bônus e Participação nos
Resultados 5114 Despesas com Ajustes de Acordo Comercial 5116 Perdas com
Devedores Duvidosos 5117 Taxa Flat de Financiamento 5118 Despesas de
Consumo Interno Oficina 5119 Fretes 5120 Despesa com reforma de máquinas
usadas 5121 Remuneração Dirigentes 5122 Salários - Gerência e Supervisão
5123 Salários - Funcionários 5124 Provisões - 13o. Sal. e Férias 5125
Encargos Sociais - todos 5126 Benefícios 5127 Água, Luz e Gás 5128
Aluguel 5129 Amortizações e Depreciações 5130 Despesa com Alimentação
5131 Despesas com Material de Segurança e Uniformes 5132 Despesas com
Móveis e Utensílios 5133 Despesas de Comunicação 5134 Donativos e
Contribuições 5135 Eventos e Confraternização 5136 Garantias Recusadas -
AGCO 5137 Impostos e Taxas Diversos 5138 Leasing 5139 Legais e Auditoria

------------------------------------------------------------------------

## Página 11

5140 Manutenção de Veículos frota da Adm 5141 Manutenção Predial 5142
Materiais Diversos / Escritório 5143 Multas 5144 Processamento Dados -
Terceiros 5145 Propaganda e Promoção de Vendas 5146 Publicações,
Assinaturas e Assoc. 5147 Retorno de Garantia ( Pecas e Serviços ) 5148
Seguros 5149 Serviços Profissionais - Terceiros 5150 Treinamento 5151
Combustíveis e Lubrificantes 5152 Viagens, Represent. e Hospitalidade
5153 Taxas Associativas 5154 Sistemas, Software e Informática 5155
Postagens, Correios e Telégrafos 5156 Aviso Prévio 5157 Ações
Trabalhistas 5158 Estágio 5199 Outros Despesas 6111 Rendimento
Aplicações Financeiras 6112 Juros Recebidos 6113 Variações Monetárias
Ativas 6114 Descontos Obtidos 6115 Juros de CAR 6121 Despesas
Financeiras s/ Capital Giro 6122 IOF 6123 Despesa com Juros 6124 Multas
Financeiras 6125 Juros sobre Capital Próprio 6126 Variações Monetárias
Passivas 6127 Despesas Bancárias 6128 Descontos Passivos 7111 Ganho com
Venda de Ativo Imobilizado 7112 Outras Receitas 7121 Perda com Venda de
Ativo 7122 Outras Despesas 8111 IRPJ 8112 CSLL Contas Gerencias 9111
Diretores

------------------------------------------------------------------------

## Página 12

9112 Gerentes 9113 Supervisores 9114 Coordenadores 9115 Vendedores 9116
Analistas 9117 Assistentes 9118 Auxiliares 9119 Diretores 9211 Novos
9212 Usados 9213 Outros Negócios 9311 Trator 9312 Colheitadeira 9313
Colhedora 9314 Plantadeira 9315 Pulverizador 9316 Plataforma 9317
Implementos 9318 Outros 9411 Trator 9412 Colheitadeira 9413 Colhedora
9414 Plantadeira 9415 Pulverizador 9416 Plataforma 9417 Implementos 9418
Outros 9419 Colheitadeira 9420 Colhedora 9421 Plantadeira 9422
Pulverizador 9423 Plataforma 9424 Implementos 9511 Horas Disponíveis
9512 Horas Faturadas 9513 Tempo de O.S. 9514 Valor O.S. em Aberto 9611
Tempo de Pedido 9612 Horas Faturadas 9613 Valor Pedidos Abertos

------------------------------------------------------------------------

## Página 13

9.  Histórico das versões Data Versão Funcionalidades  19/04/2021 4.1
    Reestruturação de tópicos Maior detalhamento das regras de negócio.
    20/03/2021 4.0 Inserido regras de validação 22/10/2020 3.0 Adição
    dos planos de contas 20/07/2020 2.0 Agrupamento das filiais
    obrigatório - a requisição se não contiver todas as filiais
    agrupadas na mesma requisição. Adição do campo "protocol" Adição da
    rejeição de requisições maiores que 4MB. 30/11/2019 1.0 Versão
    inicial
