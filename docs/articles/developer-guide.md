# Bonsai.ML - Developer Guide

## Overview

The `Bonsai.ML` codebase is split up into 2 main repositories on GitHub. The main source code repo is contained within the [main Bonsai.ML](https://github.com/bonsai-rx/machinelearning) repo. This repo contains the source code, CI/CD pipeline, unit tests, and automatic documentation generation. There is another repo specifically containing the [Bonsai.ML examples](https://github.com/bonsai-rx/machinelearning-examples) , which allows us to maintain a clean separation between the source code and Bonsai workflows for using the package.

## Dependencies

You will need to have all of the following tools and packages installed:

- dotnet-sdk (at least version 8)
- git
- uv Python package manager

## Tooling

- VS code
  - C# Dev Kit
  - EditorConfig
- Linux - Wine v11.2
  - See here: https://ncguilbeault.com/blog/wine-bonsai
  - For Python, install Python as you would normally on Windows except use Wine to run the Python installer (e.g. `wine Python.exe`)

## Building source code

The typical development workflow is the following.

1. clone the repo and make sure submodules are updated

```bash
git clone --recursive https://github.com/bonsai-rx.org/machinelearning.git
```

2. change to the machine learning directory and build

```bash
cd machinelearning
dotnet build . # Performs a package restore and builds the Bonsai.ML packages
dotnet test # Runs all of the tests and confirms that the code is running as expected
```

The above commands ensures that the code builds properly and that the tests run correctly. All of the build outputs are placed into a folder in the main root called `artifacts`.

## Continuous Integration and Deployment

We use an automated CI/CD pipeline to ensure the `Bonsai.ML` source code is built correctly, passes all unit tests, and documentation gets generated correctly. The CI/CD pipeline also allows us to automatically generate new releases of `Bonsai.ML` packages with automatic [SemVer](https://semver.org/) tags which get pushed directly to [NuGet](https://nuget.org/packages?q=Bonsai.ML) . However, by default, when building Bonsai.ML packages locally, the version will be tagged with a specific version that cannot be easily confused with official package versions. For instance, building a package locally will produce the following:

```console
$ dotnet build .
$ dotnet pack .
...
Successfully created package 'artifacts/package/release/Bonsai.ML.42.42.42-dev0.nupkg'.
```

These package names can be difficult to work with. Instead, we can override the build process with specific MSBuild properties to create a local package with a more desirable name, like this:

```console
$ dotnet build .
$ dotnet pack . -p ContinuousIntegrationBuild=true -p CiBuildVersion=0.0.0-custom
Successfully created package 'artifacts/package/release/Bonsai.ML.0.0.0-custom.nupkg'.
```

The CI/CD pipeline also plays a role in automatically building the documentation website and serving it statically through GitHub pages. This uses docfx and uses both the main repo and the examples submodule. The CI/CD pipeline requires that the tests succeed, and that the documentation is built successfully.

## Contributing to Bonsai.ML

Contributing usually means adding a new library with examples and unit tests. Before you begin, ensure you can build the existing packages and run through a few of the examples. You should also ensure you can build the documentation website locally.

## Adding a new Bonsai library

Below, we've outlined the general steps one would undertake to add a new package to the Bonsai.ML repo.

1. Ensure you can build the existing repo and run through a few of the examples.
2. Ensure you can build the documentation website locally.
3. Create a new bonsai library:

```bash
dotnet new bonsailib -n Bonsai.ML.NewPackage -o src/Bonsai.ML.NewPackage
```

4. Edit the `.csproj` file of the new package. Most of the project tags will be populated automatically using the shared build tooling, but it is still important to include a few things, like a package description and package tags that are specific to this package. Any other properties that are specific to the package should go here. It's also a good idea to target both `netstandard2.0` as well as `net472` to maintain backwards compatibility for now while preparing for future Bonsai versions.
5. Add the new package to the main solution file:

```bash
dotnet sln Bonsai.ML.sln add --in-root src/Bonsai.ML.NewPackage/
```

6. Build the package.
7. Once the package is finished, make sure to update the documentation in the `docs/` folder to include any articles or installation instructions that might be important to know for the package.

## Adding a new test library

Below are the steps for adding a new testing library for a Bonsai.ML package

1. Create a new test project:

```bash
dotnet new mstest -n Bonsai.ML.NewPackage.Tests -o tests/Bonsai.ML.NewPackage.Tests
```

2. Add your tests project to the solution:

```bash
dotnet sln Bonsai.ML.sln add --in-root tests/Bonsai.ML.NewPackage.Tests/
```

3. Add your test cases.
4. Run the tests and confirm they succeed:

```bash
dotnet test .
```

### Adding Examples

Below are the steps for adding a new example:

1. Create a new folder inside of the `examples/` folder.
2. Open a terminal and navigate to the newly created example folder.
3. Create a bonsai environment inside of the example using:

```bash
dotnet new bonsaienv
```

4. Launch your newly created bonsai environment. For this, run the following:

```bash
bonsai
```

5. Install the necessary bonsai packages. If you are installing a locally built Bonsai.ML package, ensure you have added the local Bonsai.ML artifacts build path to the `NuGet.config` file inside of the `.bonsai/` folder.
6. Create the example workflow.
7. Document the example workflow in the example's `README.md` as well as any instructions needed to run the example.

## Notes on Deploying New Packages

Currently, the CI/CD pipeline handles generating docs from source code, building and running examples, and integrating both source code and examples into a unified documentation website. However, getting the CI/CD pipeline to play nicely when developing new packages and examples can be tricky. In order to successfully build the documentation, both the example workflows need to be correct and up-to-date with latest packages in the source code. However, what is most common is that you want to add a new package at the same time as you want to add new examples. Anyy PR that is made requires running through the CI/CD pipeline, which means that before a source code PR can be successfully updated to new package versions, the examples must already be merged into the examples repo (or vice versa updating source code and then making a PR to update the examples), which can lead to a temporary failure of the CI/CD pipeline.
