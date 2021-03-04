# How it works

lignator works by taking a template of how a log should be generated, then parsing all the tokens with their possible values and storing them in memroy during start up. It can then start to output the actual logs. The core benefit of this over other solutions is that it greatly reduces the compute and IO overhead during log generation, which allows us to generate a consistent and steady rate of logs for long periods of time.

lignator has a simple approach to creating logs.You can simply take an example from a real world scenario, like so:

Example Log

```
[2020-08-10 15:58:34.135] - [INFO ] - I am a log for request with id: 89be63ef-756a-44f1-87d1-1422932905c5
```

Identitfy the items which need to be generated, for this example it would be:

- timestamp
- log level
- ID

You can then replace those values from the sample log with their respective token. Below is an example. We will cover which tokens are available and how to use them later on, but for now you can see this is still fairly consumable for new people to get started.

Template

```
[%{utcnow()}%] - [%{randomitem(INFO ,WARN ,ERROR)}%] - I am a log for request with id: %{uuid}%
```

[>> Next - Inputs](/docs/input.md)