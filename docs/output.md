# Output

- [inline](#inline)
- [file](#file)
- [directory](#directory)

The output will vary depending on the input you provide. By default it will output in a directory called output (it will create this directory if it doesn't exist). If you wish to use a different directory you can change this with the output option detailed below.

## Inline

When runing lignator inline like so:

```
$ lignator -t "ID: %{uuid}%"
```

It will output all the logs into a single file "./logs/lignator.log"


## File

When running lignator with a specific file as the input, like the example below, it will use the file name to create the output file.

```
$ lignator -t ./templates/nginx.template
```

It will output all the logs into a single file "./logs/nginx.log"

## Directory

If you need to run with multiple files in the input and are running lignator with a directory as the input, it will create a log file with the same name as each file it has used from the input directory. Take a directory like the image below and the lignator command will work as follows:

![Directory sturcture example](/images/examples-directory.png)

```
$ lignator -t ./templates
```

Will output the following files:

- ./logs/aksapiserver.log
- ./logs/apache.log
- ./logs/applicationgateway.log
- ./logs/azurefirewall.log
- ./logs/linux.log
- ./logs/nginx.log

[>> Next - options](/docs/options.md)