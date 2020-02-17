## Testing

The command above gets all of the quotes history for an instrument

curl http://localhost:5000/api/instrumentsdata?name=BTC-21FEB20-9500-C

to get quotes in a date interval :

curl http://localhost:5000/api/instrumentsdata?from=1581812821793&to=1581812821799&name=BTC-21FEB20-9500-C