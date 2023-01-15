# CLI.Down

this project is a small CLI tool that downloads multiple files from various sources in parallel. 

all necessary data are extracted from a configuration file (.yaml).

Beside downloading files, the tool should verify the existence of the downloaded files.

Task 1:
Now the CLI supports parsing the following commands with their arguments (arguments order dosen't matter) :
-	download [--verbose] [--dry-run] [parallel-downloads=N] config.yml
-	validate [--verbose] config.yml
Also parsing the provider yaml file name into an object