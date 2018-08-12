# helm json plugin

Formats the output of helm commands to json.

Works for commands: `install`, `status`

Example usage:

`helm json install stable/rabbitmq`

`helm json status my-release-name`

## Installtion

`helm plugin install https://github.com/eladiw/helm-json-output --version nodejs-version`

## Installing the plugin locally

1. Clone the repo
2. Install the plugin pointing to the folder

```bash
helm plugin install .
```