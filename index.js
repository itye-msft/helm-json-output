const execFile = require('child_process').execFile;

const helmBinary = process.env.HELM_BIN;
const args = process.argv;

/**
 * Prints the extension's usage
 */
function printUsage() {
  const usage = "Converts helm's output to json format.\n"
    + 'Works for commands: install, status\n'
    + '\n'
    + 'Example usage:\n'
    + '  helm json install stable/rabbitmq\n'
    + '  helm json status my-release-name\n';
  process.stdout.write(usage);
}

// The expected arguments are:
// ['node', 'js file', 'command', 'chart / release name']
if (args.length < 4) {
  printUsage();
  process.exit(0);
}

/**
 * Builds a json object from a string array of resources.
 * format example:
 * resourcesArr = ['==> v1beta1/StatefulSet \n
                    NAME   DESIRED  CURRENT  AGE \n
                    fun-elk-mariadb  1        1        2s',
                   '==> v1/Pod(related) \n
                    NAME   READY  STATUS   RESTARTS  AGE \n
                    fun-elk-wordpress-665ff69d4b-9kg8x  0/1    Pending  0  2s \n
                    fun-elk-mariadb-0    0/1    Pending  0   2s']
 */
function ConvertToObject(resourcesArr) {
  const json = [];
  resourcesArr.forEach((element) => {
    const lines = element.split('\n');
    let name;
    const resources = [];
    lines.forEach((line) => {
      if (line.startsWith('==>')) {
        name = line.substring(4).trim();
      } else if (line.startsWith('NAME') === false) {
        resources.push(line);
      }
    });

    json.push({
      name,
      resources,
    });
  });
  return json;
}

/**
 * From the bulk resources string, parse each individual resource and insert
 * it to the resources array which is returned at the end.
 */
function ParseResources(resourcesStr) {
  const array = [];

  // Sanity
  if (resourcesStr.trim() === '') {
    return array;
  }

  let ind = 0;
  let outputCopy = resourcesStr;
  while (ind !== -1) {
    ind = outputCopy.lastIndexOf('==> ');
    array.push(outputCopy.substring(ind).trim());
    outputCopy = outputCopy.substring(0, ind);
  }

  return array;
}

/**
 * Extracts the resources section from the helm raw output.
 * The output of this method is one text bulk of ALL the resources
 */
function ExtractResources(helmRawOutput) {
  // Sanity
  if (helmRawOutput.trim() === '') {
    return '';
  }
  const matches = helmRawOutput.match(/(.*RESOURCES:\s+)((.|\n)*)(\s*NOTES:.*)/);

  if (matches.length < 3) {
    return '';
  }

  return matches[2];
}

/**
 * Extracts release name from the raw output of helm
 */
function ExtractReleaseName(helmRawOutput) {
  // Sanity
  if (helmRawOutput.trim() === '') {
    return '';
  }

  const lines = helmRawOutput.split('\n');
  const lookup = 'NAME:';
  if ((lines.length === 0) || (lines[0].startsWith(lookup) === false)) {
    return '';
  }

  return lines[0].replace(lookup, '').trim();
}

/**
 * Extract the release name, all the different resources and returns the
 * data as a json object
 */
function parseResponse(data) {
  const releaseName = ExtractReleaseName(data);
  const unFormattedResources = ParseResources(ExtractResources(data));
  const structuredResources = ConvertToObject(unFormattedResources);
  return { releaseName, resources: structuredResources };
}

// Script start:

// remove first two items n the array (which are 'node executable.js') and
// leave only the exec arguments.
args.splice(0, 2);

// Execute helm with the given arguments and parse the
// response
execFile(helmBinary, args, (err, stdout, stderr) => {
  if (err) {
    process.stderr.write(stderr);
  } else {
    const json = parseResponse(stdout);
    process.stdout.write(JSON.stringify(json));
  }
});
