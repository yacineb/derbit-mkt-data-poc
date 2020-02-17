# Deribit instruments data retrieval POC

## Ho to get the service up and running

docker-compose up -d

## REST Api

The command above gets all of the quotes history for an instrument

curl http://localhost:5000/api/instrumentsdata?name=BTC-21FEB20-9500-C

to get quotes in a date interval :

curl http://localhost:5000/api/instrumentsdata?from=1581812821793&to=1581812821799&name=BTC-21FEB20-9500-C

**The result is a json array of {t,p} object ordered by t. t stands for timestamp and p for prices**.

> If an instrument does not exist or has no quote in the given date interval then the returned array is empty

## Deribit Api client

I chose to interface with Deribit Server using websocket.

Websockets they have less overhead than http because of less client-server handshaking. 
They also allows full-duplex communication and subscriptions which is a very common in such kind of services. (Subscribe to a data feed)

## Storage

- Data retrieved from Deribit Api are stored a MongoDb. I used the official c# MongoDb driver which has a great api.
Thanks to its Linq Provider, it makes querying a bit agnostic to storage technology and query DSL.

- There is no need for transactions or relational features. Document databases are fine for append-only data insertion and for
storing time-series. 


## "Roadmap" What is would have done if i had more time

**business logic**
- Handle properly the Deribit Api error codes.
- Make data retrieval interval remotely configurable using /config endpoint.
- Make rate limitation dynamically adjustable. Accoring to this documentation https://www.deribit.com/pages/information/rate-limits , quotas are queryable by api.
They could be polled on a regular basis in order to upgrade rate limitation
- Maybe exposing a Bulk get for instruments quotes could be useful..
- The rate limiter i've implemented is for POC purpose. for production scenario we may call Deribit api from many services instances. So we need a distributed rate Limiter.
Redis is commonly used to implement rate limiters (https://redislabs.com/redis-best-practices/basic-rate-limiting/)

**other**
- Add logging : Serilog is a great library for this purpose..
- Secure the REST api using Authentication: exposed data are sensitive (especially the one under /private  route)
- user/password for database access.
