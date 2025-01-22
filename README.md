# Objetivo

Este Projeto tem o objetivo de exemplificar o uso de diversas tecnologias e padrões diferentes aplicados tanto no backend como frontend.

Você também pode ver outro projeto teste publicado usando .net, blazor server, blazor wasm, angular, nextjs, e nodejs no link: https://danillofratta-001-site1.itempurl.com/index.html

# Tecnologias aplicadas

Este projeto as seguintes tecnologias:
*	Backend DOTNET:
  *	Serviços API restful
  *	RabbitMQ
  *	Polly 
  *	SignalR 
  *	CQRS
  *	MediatR
  *	Regis 
  *	Ocelot 
  *	Entity

* Frontend Angular simples:
  * Material
  * SignalR
 
# Exemplos aplicados:

Exemplos de tecnologias e padrões aplicados:

1.	Exemplificação de uso do RABBIT + SignalR + Polly

A comunicação entre os 3 serviços pelo Rabbit:
*	Serviço Order (gerencia o pedido de  venda)
*	Serviço Stock (gerencia o estoque)
*	Serviço Sale (gerencia a venda)

Este é o fluxo de venda OK:
*	Serviço Order => cria e envia notificação para serviço stock
*	Serviço Stock => produto e quantidade OK?
  *	Envia notificação para serviço de order que altera status da order
  *	Envia notificação para serviço de sale que cria a venda 
*	Serviço Sale => payment OK
  *	Envia notificação para serviço de order que altera o status da order

Este é o fluxo da FALHA na venda:
*	Serviço Order => cria e envia notificação para serviço stock
*	Serviço Stock => produto e quantidade não OK?
  *	Envia notificação para serviço de order que altera status out of stock da order 

2.	Exemplificação de uso CQRS + MediatR + Regis

No serviço Stock foi criada a funcionalidade simples de CRUD do product usando CQRS, MediatR e Redis.

3.	Exemplificação de uso de Gateway

Foi adicionado Gateway com OCELOT para gerenciar os 3 serviços.

4.	Exemplificação de build com Docker

Foi criado uma imagem para cada app e docker compose para todo o projeto.

O objetivo é demonstrar o conhecimento e uso das tecnologias por isso não foi implementado padrões como DDD e por isso o ideal seria refatorar toda aplicação para separar as responsabilidades.
