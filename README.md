# helm json plugin
Formats the output of helm commands to json.

Works for commands: `install`, `status`

Example usage:

`helm json install stable/rabbitmq`

`helm json status my-release-name`

## Installtion
`helm plugin install https://github.com/itye-msft/helm-json-plugin --version master`

## Building the plugin and installing locally:
1. Clone the repo
2. Using the command line navigate to the cloned folder, and run:
   - `dotnet restore`
   - `dotnet build`
   - `dotnet publish -o lib -c Release`
3. Install the plugin pointing to the folder.