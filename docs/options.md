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

[>> Next - Tokens](/docs/tokens.md)