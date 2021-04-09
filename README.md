# lignator

![Build and Tests](https://github.com/microsoft/lignator/workflows/Build%20and%20Tests/badge.svg) ![Release](https://github.com/microsoft/lignator/workflows/Release/badge.svg)

## Log generation made simple

lignator (Latin for lumberjack) is a cli tool to generate structured but randomized outputs. It was originally created for generating event logs to help test Splunk clusters at scale, but as it has evolved we are seeing it being used to generate other randomized structures such as SQL queries, CSV or json. Make sure you check out the examples for some ideas and feel free to contribute some new ones.

![lignator demo](/images/lignator-demo.gif)

# Getting Started

Head over to [releases](https://github.com/microsoft/lignator/releases) and grab the latest version, all the details on how to install it will be in the release notes.

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
# Tokens

- [uuid](#uuid)
- [randomitem](#randomitem)
- [randombetween](#randombetween)
- [linefromfile](#linefromfile)
- [utcnow](#utcnow)

The current tokens available are shown here:

| Token         | Description                                  | Example                                                          |
| ------------- | -------------------------------------------- | ---------------------------------------------------------------- |
| uuid          | Creates a new uuid per log                   | lignator -t "request id: %{uuid}%"                                 |
| randomitem    | Picks a random item from those provided      | lignator -t "LogLevel: %{randomitem(INFO , WARN , ERROR)}%"        |
| randombetween | Picks a random number between those provided | lignator -t "A Number between 1 and 10 = %{randombetween(1,10)}%"   |
| linefromfile  | Picks a random item from a file              | lignator -t "linefromfile: %{linefromfile(filepath)}%"             |
| utcnow        | Generate timestamp using UTC                 | lignator -t "timestamp: %{utcnow()}%"                              |
| variable      | Uses the result of a already transformed template so you can re use it per output | lignator -V myID=%{uuid}% -t "ID:%{variable(myID)}% the same ID: %{variable(myID)}%" |

## uuid

The uuid token is used to create a new unique id.

![uuid generation example](/images/lignator-uuid.gif)

## randomitem

The randonitem token can take a group of values inline, so it's good for when you are only working with a handful of items. Here is an example of generating logs with different log levels:

> Note in this example the space between some of the items is to ensure padding of the values in the log.

![randomitem example](/images/lignator-randomitem.gif)

## randombetween

The randombetween token allows lignator to generate a random number between and inclusive of the provided numbers. So in the example below it will create a number that is greater than 0 and less than 11.

```
$ lignator -t %{randombetween(1, 10)}%
```

![randombetween example](/images/lignator-randombetween.gif)

## linefromfile

The linefromfile token will let lignator pick at random a line from the supplied file. This is helpful when you have so many items that the randomitem becomes challegening to maintain or consume. A great example could be a file filled with browser agents.

![linefromfile example](/images/lignator-linefromfile.gif)

## utcnow

The utcnow token allows lignator to generate a range of different timestamps based on the current UTC date and time. Under the hood, it currently uses the .NET implementation of DateTime so you can use the details [here](https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings) for examples of all the available format strings.

> The utcnow function has an optional parameter allowing you to set the output format. By default it is "yyyy-MM-dd HH:mm:ss.fff"

Here are a few examples:

```
$ lignator -t "%{utcnow(yyyy-MM-dd)}%"
```

2021-02-10

```
$ lignator -t "%{utcnow(yyyy-MM-dd'T'HH:mm:ss.ffffff'Z')}%"
```
2021-02-10T04:32:50.540265Z

![utcnow example](/images/lignator-utcnow.gif)

## variable

The variable token allows lignator to evaluate a specific template and store the result to re use multiple times in a single log or output, this could be handy for when you want to output the same value in something like a firewall log or include the same IP in the url and host values of a http request header.

> Keep in mind that when lignator is in its default mode or transforming single line templates the variable will be in scope per line, when in multiline mode the variable will be in scope across lines but will within the scope of a single template transformation.

Here are a few examples:

```
$ lignator -V myID=%{uuid}% -t "MyID: %{variable(myID)}% Your ID isn't: %{variable(myID)}%" -l 10
```

You can see here that the id is re-used per line but then the variable is re-evaluated per log or output.

```
MyID: 5aaee6ef-35c8-4be3-b870-9439c3d03313 Your ID isn't: 5aaee6ef-35c8-4be3-b870-9439c3d03313
MyID: ea392ee7-1845-4b2d-9535-96a86072c1a8 Your ID isn't: ea392ee7-1845-4b2d-9535-96a86072c1a8
MyID: 79c5f880-5f80-465d-aaa3-1cfc19aa213d Your ID isn't: 79c5f880-5f80-465d-aaa3-1cfc19aa213d
MyID: 2e0cb86e-619f-4e53-b19b-34cf83e39294 Your ID isn't: 2e0cb86e-619f-4e53-b19b-34cf83e39294
MyID: 258f513a-7550-48bf-8fe3-73fc0c5ccd7b Your ID isn't: 258f513a-7550-48bf-8fe3-73fc0c5ccd7b
MyID: f066def1-a825-4fd7-90ae-dcd7253562aa Your ID isn't: f066def1-a825-4fd7-90ae-dcd7253562aa
MyID: 4e82f985-d76c-4b51-814f-28a98f495ef4 Your ID isn't: 4e82f985-d76c-4b51-814f-28a98f495ef4
MyID: 255339da-1724-4965-a0ed-1def22719f31 Your ID isn't: 255339da-1724-4965-a0ed-1def22719f31
MyID: 59a381b7-9126-48d2-b298-6c1f10d3acae Your ID isn't: 59a381b7-9126-48d2-b298-6c1f10d3acae
MyID: aa529347-8167-4068-acd5-3587d6916e52 Your ID isn't: aa529347-8167-4068-acd5-3587d6916e52
```

Multiline example

Using a input file like:

```
{
  "myID":"%{variable(myID)}%",
  "child": {
    "parentId":"%{variable(myID)}%"
  }
}
```

> the key here being the multiline argument being set to true

```
$ lignator -V myID=%{uuid}% -t ./variableexample.json -m true -e json
```

Generates

```
{
  "myID":"1f68bb63-bebb-44a4-b442-cf288e8d07e3",
  "child": {
    "parentId":"1f68bb63-bebb-44a4-b442-cf288e8d07e3"
  }
}
```

# Input

When using lignator there are a few ways to set the template(s) it should use when running. You can use inline for passing a single template which allows you to generate a range of logs based on one template. You can pass a file which contains one or more templates. This is good if you want a simple way to create logs in different systems as part of a single run or a system that can output different formats into a single log file. The final approach is using a directory. This allows you to supply a whole directory of files which, like the previous example, can contain a single template or multiple templates per file. Check out the next section for more details on each.

## Inline

Inline is the simplist way to get started with lignator.You can simply pass a template inline and don't need to worry about creating templates and saving them to disk. We will cover the types of templates you can create later, but using the example we have seen already, we could run it like so:

```
$ lignator -t "[%{utcnow()}%] - [%{randomitem(INFO ,WARN ,ERROR)}%] - I am a log for request with id: %{uuid}%"
```

This, with the default options set, will output a single log into the output destination.

## File

You can supply template(s) to lignator via a file. It may be that you want to capture the template in source control or that you need to generate different logs into a single output in a similar way to some systems. Lets take the following as an example:

```
%{utcnow(MMM dd HH:mm:ss.ffffff)}% combo sshd(pam_unix)[%{randombetween(10000,30000)}%]: check pass; user %{linefromfile(./taxonomies/usernames.txt)}%
%{utcnow(MMM dd HH:mm:ss.ffffff)}% combo sshd(pam_unix)[%{randombetween(10000,30000)}%]: authentication failure; logname= uid=%{randombetween(0,255)}% euid=0 tty=NODEVssh ruser= rhost=%{randombetween(1,254)}%.%{randombetween(1,254)}%.%{randombetween(1,254)}%.%{randombetween(1,254)}%  user=%{linefromfile(./taxonomies/usernames.txt)}%
%{utcnow(MMM dd HH:mm:ss.ffffff)}% combo su(pam_unix)[%{randombetween(10000,30000)}%]: session opened for user %{linefromfile(./taxonomies/usernames.txt)}% by (uid=%{randombetween(0,255)}%)
%{utcnow(MMM dd HH:mm:ss.ffffff)}% combo su(pam_unix)[%{randombetween(10000,30000)}%]: session closed for user %{linefromfile(./taxonomies/usernames.txt)}%
%{utcnow(MMM dd HH:mm:ss.ffffff)}% combo sshd(pam_unix)[%{randombetween(10000,30000)}%]: authentication failure; logname= uid=%{randombetween(0,255)}% euid=%{randombetween(0,255)}% tty=NODEVssh ruser= rhost=%{randombetween(1,254)}%.%{randombetween(1,254)}%.%{randombetween(1,254)}%.%{randombetween(1,254)}%
%{utcnow(MMM dd HH:mm:ss.ffffff)}% combo logrotate: ALERT exited abnormally with [%{randombetween(1,255)}%]
```
If these were stored in a file called "linux.template" in the current directory you could run the following:

```
$ lignator -t ./linux.template -l 100
```

lignator would pick lines at random 100 times, transform the tokens and output the logs to the destination.

> While you would be guaranteed to get 100 lines, lignator is picking them at random so it is possible that not all the templates in the file will be used.

## Directory

Building on the previous examples, we could now build out a structure or templates for a range of systems and log types and have the logs easily generated as part of a single lignator execution. Let us take the following directory structure as an example:

> When working with a directory as the input, it will scan the supplied directory for all files with the extension .template

![Directory sturcture example](/images/examples-directory.png)

If this directory was stored within the current directory, we could run the following:

```
$ lignator -t ./templates -l 100
```

lignator would then randomly pick a file and then randomly pick a line from that file and transform it into a log until it had generated 100 logs into the output.

## Header

The header is a way to add something at the start of the file, this could be anything from static headers for a csv file or an input file with multiple lines and it's own tokens for transformation.

> Important note: When using the "--head" argument you can only pass it a template inline or via a specfic file

Here is a basic example for a csv file. This would generate a csv file with 2 lines, the first being the titles ID, Timestamp and Value. The second line would be the output of the tokens in the template.

```
$ lignator -H "ID,Timestamp,value" -t "%{uuid}%,%{utcnow()}%,%{randomitem(value1,value2)}%" -e csv
```

A more complex example could be a http request based on the following files and multiline inputs and outputs using the "-m" flag:

header.template

```
%{randomitem(POST,PUT)}% https://%{randombetween(1,254)}%.%{randombetween(1,254)}%.%{randombetween(1,254)}%.%{randombetween(1,254)}% HTTP/1.1
Content-Type: application/json

```

body.template

```
{
  "id": "%{uuid}%",
  "nested:" [
    {
      "%{utcnow}%"
    }
  ],
  "value": "%{randomitem(value1,value2)%"
}
```

```
$ lignator -H ./header.template -t body.template -e http -m true
```


## Template

The template is the core of lignator, this is where you can mix plaintext with tokens described in the documentation to generate most things you can imagine. Something simple like a CSV to something more complex like a http request, which we saw in the previous example.

## Tail

Just like the header argument we have alredy looked at the "--tail" argument is just the same, with the only difference being that it adds something to the end of the file rather than the start.

> Important note: When using the "--tail" argument you can only pass it a template inline or via a specfic file

## Variables

There are mainy cases where you may need to randomly generate a value and then re-use that value in a transformation of a template. To help with this we have added the ability to define variables. Setting them from the cli would look like:

```
$ lignator --variable myid=%{uuid}% --variable currenttime=%{utcnow()}%
```

As you can see right away, these are not statically defined values but are also in the form of a template which can contain one or more tokens. You can then use the variable token passing it the name of the variable you wish yo use.

```
$ lignator --variable myid=%{uuid}% -t "ID:%{variable(myid)}% - Yes the ID was: %{variable(myid)}%"
```

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

# Options

These are the different values that can be configured for when lignator is run. The only manditory item is the template which can be inline, a path to a file and a path to a directory. The rest have default values so while likely to be changed they don't have to be for small test runs.

As well as being able to pass configuration items in via the cli, you can also set them through environment variables. This makes it easier when running inside a container and inside kubernetes. We will see examples of this later.

Here are the details of the cli arguments, aliases and their matching environment variables name. The way lignator checks for theses is as follows:

- cli argument
    - environment variable
        - default
            - error

> So with this hierachy in mind, if you supplied the template as both a cli argument and an environment variable, it would use the cli argunment.

> You don't have to provide all the items as either cli arugments or environment variables. You can mix and match them as needed, as long as you keep in mind the hierachy shown above.

| Option       | Alias | Env Variable       | Required | Default           | Type    | Description                                                                              |
| ------------ | ----- | ------------------ | -------- | ----------------- | ------  | ---------------------------------------------------------------------------------------- |
| --template   | -t    | lignator_template  | Yes      |                   | Text    | The representation of a log line, that can contain tokens that will be replaced per line, or a file which contains one or more templates per line |
| --runs       | -r    | lignator_runs      | No       | 1                 | Number  | If not supplied it will only run once, otherwise this will specify how many times to run |
| --logs       | -l    | lignator_logs      | No       | 1                 | Number  | The number of logs per run to create                                                     |
| --output     | -o    | lignator_output    | No       | ./logs            | Text    | The directory you would like the logs to be put                                          |
| --clean      | -c    | lignator_clean     | No       | False             | Boolean | Delete the files at the end of each run                                                  |
| --infinite   | -i    | lignator_infinite  | No       | False             | Boolean | Whether to run continuously                                                              |
| --extension  | -e    | lignator_extension | No       | log               | Text    | The file extension used when generating the file outputs                                 |
| --multi-line | -m    | lignator_multiline | No       | False             | Boolean | When used with an input file or directory, tells lignator to treat the file as a single input for transforming |
| --head       | -H    | lignator_head      | No       |                   | Text    | Add content to the start of the file                                                     |
| --tail       | -T    | lignator_tail      | No       |                   | Text    | Add content to the end of the file                                                       |
| --variable   | -V    | lignator_variables | No       |                   | Text    | Define a variable which will be evaluated per execution, i.e. per line or multiline log  |
| --version    |       |                    | No       |                   |         | Shows current version of tool                                                            |
| --help       | -h    |                    | No       |                   |         | Shows the help text for the tool                                                         |
| --no-banner  |       | lignator_no_banner | No       | False             | Boolean | Use this argument to not show the ascii banner |

# Considerations
## Continuous / multiple runs

When running with more than one run, lignator will extract and generate the required details for all the tokens in use and store them in memory and then re-use them for each run. To ensure there is no conflict with the output at the end of each run, you can set the clean flag to remove the logs from the previous run (otherwise it will continue to append the logs to the existing file).

If you set lignator to run using the infinite option, it will force lignator to run continuously until the process is forceably stopped. This can be handy if you need to run it in kubernetes to create a long term generation of logs.

## Nested tokens

At the moment a token can only accept simple types for their inputs. We are currently considering the best approach, if at all, to add support for nested tokens. The idea being you could do something like:

```
$ lignator -t "%{linefromfile(randomitem(file1.txt, file2.txt))}%"
```

# Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

# Trademarks

This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft
trademarks or logos is subject to and must follow
[Microsoft's Trademark & Brand Guidelines](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general).
Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft sponsorship.
Any use of third-party trademarks or logos are subject to those third-party's policies.
