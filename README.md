# ItaimBibiGasPizzaBot

## Descrição

presentation/p01

	- criação do projeto no vs2017 community
	- configuração do projeto para .NET 4.6.1
	
presentation/p02

	- instalação do pacote Takenet.MessagingHub.Client.Template
	
presentation/p03

	- instalação do simple injector e criação do service provider => boa prática => injeção de dependência
	- criação de receiver base para todos os outros receivers => boa prática => implementação de regras de negócio genéricas para todos os receivers
	- uso da extensão de contato + limeexception resource not found
	- uso da extensão de diretório
	- envio de mensagens + originador da mensagem (envelope.From)
	
presentation/p04

	- criação de receiver de localização (filtro de mediatype)
	- criação de receivers para informar preço ou pedir preço (filtro de content e priority)
	- criação do contact service (injeção de depêndencia usando service provider)
	- uso da extensão de recursos para pegar a mensagem de boas vindas
	- criação de um menu quick reply com opções
	
presentation/p05

	- uso do bucket para criar dados persistentes entre interações

presentation/p06

	- criação de um serviço fake para os postos de gasolina (uso do startup para inicializar dados)
	- criação de um carrossel com imagens e botões (texto e compartilhar)
	- uso de state para "prender" um usuário num fluxo