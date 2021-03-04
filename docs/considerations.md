# Considerations
## Continuous / multiple runs

When running with more than one run, lignator will extract and generate the required details for all the tokens in use and store them in memory and then re-use them for each run. To ensure there is no conflict with the output at the end of each run, you can set the clean flag to remove the logs from the previous run (otherwise it will continue to append the logs to the existing file).

If you set lignator to run using the infinite option, it will force lignator to run continuously until the process is forceably stopped. This can be handy if you need to run it in kubernetes to create a long term generation of logs.

## Nested tokens

At the moment a token can only accept simple types for their inputs. We are currently considering the best approach, if at all, to add support for nested tokens. The idea being you could do something like:

```
$ lignator -t "%{linefromfile(randomitem(file1.txt, file2.txt))}%"
```