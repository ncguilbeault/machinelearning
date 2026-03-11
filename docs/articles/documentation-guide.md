# Bonsai.ML - Documentation Guide

## Overview

The documentation in the [Bonsai.ML repo](https://github.com/bonsai-rx/machinelearning) is directly tied to the [Bonsai.ML website](https://bonsai-rx.org/machinelearning) that gets generated using [docfx](https://dotnet.github.io/docfx/). Using docfx allows us to build the documentation in the form of Markdown articles. It also allows us to build the API directly from the source code, and gives us flexibility to embed custom articles and Bonsai workflows into the website.

## Building the documentation

The first thing to do is make sure the [docfx](https://dotnet.github.io/docfx/) tool is installed. To do this, it should be possible to do the following:

```bash
dotnet tool install docfx
```

Then, once installed, you can run the following from the root of the main [Bonsai.ML repo](https://github.com/bonsai-rx/machinelearning):

```bash
dotnet docfx --serve docs/docfx.json
```

Once this is complete, you should see a local development server running and accessible on http://localhost:8080.

## Structure

### Main README

The [Bonsai.ML](https://github.com/bonsai-rx/machinelearning) repo contains a main `README.md` file in the docs folder, which serves as the content of the main GitHub page as well as the website's landing page. We use this to briefly describe the different packages contained in Bonsai.ML and their functionality.

### Docs folder

The docs folder contains docfx configuration files and articles that get built into the documentation website.

```
├── docs/
	├── examples/     # Submodule for Bonsai.ML examples
	├── bonsai-docfx/ # Submodule containing template for Bonsai documentation
	├── articles/     # Articles for specific packages
	├── workflows/    # Bonsai workflows that can be referenced in articles
	├── toc.yml       # Main TOC for linking articles, API, and exampes
	├── README.md     # Main README which appears on GitHub and landing page
	├── ...           # Other configuration files
```

## Articles

The articles provide additional context or information relating to specific packages. This can include an overview of how the package might be typically used, or common programming patterns that we've identified. It can also include important information about how the package actually works under the hood, information which would otherwise be too much detail for a specific example or too high-level for the C# API reference. We also use these articles to reinforce the general installation steps that might be needed for using the package, such as installing Python virtual environments or selecting a specific backend (e.g. CPU or GPU) for hardware processing.

### Workflows

The workflows folder gives us a place to include workflows that we want to embed inside of an article. Again, these workflows may not be full functional, but provide a chance to highlight the general structure or key components of a package that will show up in different examples or applications.

## Examples

The [Bonsai.ML Examples](https://github.com/bonsai-rx/machinelearning-examples) repo also contains documentation that is specifically configured to interact with the [Bonsai.ML](https://github.com/bonsai-rx/machinelearning) main repo.

### Structure

```
├── datasets/         # A common folder to contain datasets for examples
├── docs/
	├── toc.yml       # Contains links to each individual example and categorizes them into different package applications
├── examples/
	├── LinearDynamicalSystems/ # Specific package
		├── README.md     # Provides an overview for how to install and run examples that use this package
		├── Kinematics/   # Category of applications
			├── SimulatedData/ # Specific example
				├── README.md  # Description of example and how it works
				├── workflow.bonsai # Bonsai workflow for example
				├── ...        # Other files needed to run example
		├── ...
	├── HiddenMarkovModels/    # Another package/application
		├── ...
	├── ...
```

### Main README

The main `README.md` in the root of the repo provides an overview of the examples and the different datasets that are used to run them.

### Datasets

The examples we have provided are designed to work out-of-the-box, meaning anyone can clone the examples repo and run them regardless of hardware or input devices. Thus, we provide examples with datasets that are designed to emulate streaming in data online. These shared datasets are placed into this folder, and examples that depend on these datasets should use relative paths to point to their location. It's best to keep the instructions for manipulating/changing the data as minimal as possible, since requiring users to unzip, move files, etc. can often lead to failures in resolving the correct paths automatically. The best option is to build a custom loader that can extract the data directly from compressed files such as `tar` or `zip` files.

### Table of Contents

The `toc.yml` file contained within the `docs/` folder is essential for docfx to correctly detect and build each example into the website. If an example is added to the repo but it is not included in the `toc.yml`, then it will not be accessible through the website. If adding an example to the repo, make sure it contains a descriptive `README.md` in the example folder and is properly referenced in the `toc.yml` file.

### Examples

Each example should be fully self-contained and organized into descriptive folders within specific categories. In general, we try to maintain one workflow to one example. Each example should contain all of the instructions needed for running the example. These instructions should be included in `README.md` file that lives directly inside the example repo and next to the example Bonsai workflow. Sometimes, a single package or application may have multiple examples that are similar to one another in their installation steps/structure. In these cases, it can be useful to include a `README.md` referenced in folder above the current example folder, as these will often contain instructions that relate to multiple examples.

Example `README.md` files should at least contain an overview of what the example's purpose is, a reference to the specific workflow file with a link to embed the workflow into the documentation site, and a description of what the different parts of the workflow are doing and what the expected output should be. Any additional steps needed for running the example (i.e. creating a Python environment, installing packages, downloading datasets, etc.) need to be clearly laid out or referenced in this file. It is also common to include a demo of the workflow in action, which can take the form of a GIF or a video uploaded to an external site. If a GIF is included in the example, it should be compressed to retain a small size in the repo (<1 MB), as we do not want to bloat the git history of the repo unnecessarily.

## Additional Notes

- Take extra care to use relative paths when referencing files/articles within the examples. This also includes references in the `README.md` files.
