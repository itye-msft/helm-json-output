# helm json plugin
Converts helm's output to be in json format

Works for commands: `install`, `status`

Example usage:

`helm json install stable/rabbitmq`

`helm json status my-release-name`

## Installtion
`helm plugin install https://github.com/itye-msft/helm-json-plugin --version master`

## Building the plugin and installing locally:
1. Clone the repo
2. Using the command line navigate to the cloned folder, and run:
3. `dotnet restore`
4. `dotnet build`
5. `dotnet publish -o lib -c Release`
6. Install the plugin pointing to the folder.