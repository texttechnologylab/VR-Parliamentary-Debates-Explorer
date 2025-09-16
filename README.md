<<<<<<< HEAD
[![Paper_HCII](http://img.shields.io/badge/paper-HCII--2023-B31B1B.svg)](https://doi.org/10.1007/978-3-031-35741-1_39)
[![Paper_HT](http://img.shields.io/badge/paper-HT--2023-F31B1B.svg)](https://doi.org/10.1145/3603163.3609076)
[![Conference](http://img.shields.io/badge/conference-HCII--2023-4b44ce.svg)](https://2023.hci.international/)
[![version](https://img.shields.io/github/license/texttechnologylab/Va.Si.Li-Lab)]()
![GitHub release (with filter)](https://img.shields.io/github/v/release/Texttechnologylab/Va.Si.Li-Lab)

# Va.Si.Li-Lab
a **V**R-L**a**b for **Si**mulation-based **L**earn**i**ng

# Abstract
Va.Si.Li-Lab was established as part of the project "Digital Teaching and Learning Lab" (DigiTeLL) at the Goethe University Frankfurt. 



# Va.Si.Li-Lab - Team
* Prof. Dr. Alexander Mehler (Leader)
* Giuseppe Abrami
* Mevlüt Bagci
* Dr. Alexander Henlein
* Patrick Schrottenbacher
* Christian Spiekermann

# Installation
## Server
TODO

## Open in unity
* https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022
* https://developer.oculus.com/downloads/package/meta-avatars-sdk/

## Unity
Add the git package in the Unity Package Manager
```
https://github.com/texttechnologylab/Va.Si.Li-Lab.git#upm
```
### Quick-start
1. Import the samples from the Unity package and open the `Start` scene.
2. Open the `Social Network Scene` prefab and configure the `Connection Defintion` in the RoomClient to point to your own Ubiq-Server installation, or for quick testing you can switch it out to the `Nexus Connection Definition` which is the server provided by the UCL (some features will be missing)
3. Configure the api url to point to your server in the `Scene Manager` script located in the `SceneManager` GameObject.
4. Click play!

### Advanced
1. Create a new scene to act as your starting scene.
2. Import the `Player`, `Scene Selecter` and `Social Network Scene` prefab into your scene.
3. Create another scene and add it to your build settings as well as the database.
4. Click play!


# Cite
If you want to use the project or the corpus, please quote this as follows:

Alexander Mehler, Mevlüt Bagci, Alexander Henlein, Giuseppe Abrami, Christian Spiekermann, Patrick Schrottenbacher, Maxim Konca, Andy Lücking, Juliane Engel, Marc Quintino, Jakob Schreiber, Kevin Saukel and Olga Zlatkin-Troitschanskaia. (2023). "A Multimodal Data Model for Simulation-Based Learning with Va.Si.Li-Lab." Digital Human Modeling and Applications in Health, Safety, Ergonomics and Risk Management, 539–565. [[LINK](https://doi.org/10.1007/978-3-031-35741-1_39)]

Giuseppe Abrami, Alexander Mehler, Mevlüt Bagci, Patrick Schrottenbacher, Alexander Henlein, Christian Spiekermann, Juliane Engel and Jakob Schreiber. (2023). "Va.Si.Li-Lab as a Collaborative Multi-User Annotation Tool in Virtual Reality and Its Potential Fields of Application." Proceedings of the 34th ACM Conference on Hypertext and Social Media. [[LINK](https://doi.org/10.1145/3603163.3609076)] [[PDF](https://dl.acm.org/doi/pdf/10.1145/3603163.3609076)]

# BibTeX
```
@inproceedings{Mehler:et:al:2023:a,
  author    = {Mehler, Alexander and Bagci, Mevl{\"u}t and Henlein, Alexander
               and Abrami, Giuseppe and Spiekermann, Christian and Schrottenbacher, Patrick
               and Konca, Maxim and L{\"u}cking, Andy and Engel, Juliane and Quintino, Marc
               and Schreiber, Jakob and Saukel, Kevin and Zlatkin-Troitschanskaia, Olga},
  abstract  = {Simulation-based learning is a method in which learners learn
               to master real-life scenarios and tasks from simulated application
               contexts. It is particularly suitable for the use of VR technologies,
               as these allow immersive experiences of the targeted scenarios.
               VR methods are also relevant for studies on online learning, especially
               in groups, as they provide access to a variety of multimodal learning
               and interaction data. However, VR leads to a trade-off between
               technological conditions of the observability of such data and
               the openness of learner behavior. We present Va.Si.Li-Lab, a VR-L
               ab for Simulation-based Learn ing developed to address this trade-off.
               Va.Si.Li-Lab uses a graph-theoretical model based on hypergraphs
               to represent the data diversity of multimodal learning and interaction.
               We develop this data model in relation to mono- and multimodal,
               intra- and interpersonal data and interleave it with ISO-Space
               to describe distributed multiple documents from the perspective
               of their interactive generation. The paper adds three use cases
               to motivate the broad applicability of Va.Si.Li-Lab and its data
               model.},
  address   = {Cham},
  booktitle = {Digital Human Modeling and Applications in Health, Safety, Ergonomics and Risk Management},
  editor    = {Duffy, Vincent G.},
  isbn      = {978-3-031-35741-1},
  pages     = {539--565},
  publisher = {Springer Nature Switzerland},
  title     = {A Multimodal Data Model for Simulation-Based Learning with Va.Si.Li-Lab},
  year      = {2023},
  doi       = {10.1007/978-3-031-35741-1_39}
}

@inproceedings{Abrami:et:al:2023,
  author    = {Abrami, Giuseppe and Mehler, Alexander and Bagci, Mevl\"{u}t and Schrottenbacher, Patrick
               and Henlein, Alexander and Spiekermann, Christian and Engel, Juliane
               and Schreiber, Jakob},
  title     = {Va.Si.Li-Lab as a Collaborative Multi-User Annotation Tool in
               Virtual Reality and Its Potential Fields of Application},
  year      = {2023},
  isbn      = {9798400702327},
  publisher = {Association for Computing Machinery},
  address   = {New York, NY, USA},
  url       = {https://doi.org/10.1145/3603163.3609076},
  doi       = {10.1145/3603163.3609076},
  abstract  = {During the last thirty years a variety of hypertext approaches
               and virtual environments -- some virtual hypertext environments
               -- have been developed and discussed. Although the development
               of virtual and augmented reality technologies is rapid and improving,
               and many technologies can be used at affordable conditions, their
               usability for hypertext systems has not yet been explored. At
               the same time, even for virtual three-dimensional virtual and
               augmented environments, there is no generally accepted concept
               that is similar or nearly as elegant as hypertext. This gap will
               have to be filled in the next years and a good concept should
               be developed; in this article we aim to contribute in this direction
               and also introduce a prototype for a possible implementation of
               criteria for virtual hypertext simulations.},
  booktitle = {Proceedings of the 34th ACM Conference on Hypertext and Social Media},
  articleno = {22},
  numpages  = {9},
  location  = {Rome, Italy},
  series    = {HT '23}
}

```

=======
# Bundestag Frontend



## Getting started

To make it easy for you to get started with GitLab, here's a list of recommended next steps.

Already a pro? Just edit this README.md and make it your own. Want to make it easy? [Use the template at the bottom](#editing-this-readme)!

## Add your files

- [ ] [Create](https://docs.gitlab.com/ee/user/project/repository/web_editor.html#create-a-file) or [upload](https://docs.gitlab.com/ee/user/project/repository/web_editor.html#upload-a-file) files
- [ ] [Add files using the command line](https://docs.gitlab.com/ee/gitlab-basics/add-file.html#add-a-file-using-the-command-line) or push an existing Git repository with the following command:

```
cd existing_repo
git remote add origin http://lehre.gitlab.texttechnologylab.org/Bundan/bundestag-frontend.git
git branch -M main
git push -uf origin main
```

## Integrate with your tools

- [ ] [Set up project integrations](http://lehre.gitlab.texttechnologylab.org/Bundan/bundestag-frontend/-/settings/integrations)

## Collaborate with your team

- [ ] [Invite team members and collaborators](https://docs.gitlab.com/ee/user/project/members/)
- [ ] [Create a new merge request](https://docs.gitlab.com/ee/user/project/merge_requests/creating_merge_requests.html)
- [ ] [Automatically close issues from merge requests](https://docs.gitlab.com/ee/user/project/issues/managing_issues.html#closing-issues-automatically)
- [ ] [Enable merge request approvals](https://docs.gitlab.com/ee/user/project/merge_requests/approvals/)
- [ ] [Set auto-merge](https://docs.gitlab.com/ee/user/project/merge_requests/merge_when_pipeline_succeeds.html)

## Test and Deploy

Use the built-in continuous integration in GitLab.

- [ ] [Get started with GitLab CI/CD](https://docs.gitlab.com/ee/ci/quick_start/index.html)
- [ ] [Analyze your code for known vulnerabilities with Static Application Security Testing (SAST)](https://docs.gitlab.com/ee/user/application_security/sast/)
- [ ] [Deploy to Kubernetes, Amazon EC2, or Amazon ECS using Auto Deploy](https://docs.gitlab.com/ee/topics/autodevops/requirements.html)
- [ ] [Use pull-based deployments for improved Kubernetes management](https://docs.gitlab.com/ee/user/clusters/agent/)
- [ ] [Set up protected environments](https://docs.gitlab.com/ee/ci/environments/protected_environments.html)

***

# Editing this README

When you're ready to make this README your own, just edit this file and use the handy template below (or feel free to structure it however you want - this is just a starting point!). Thanks to [makeareadme.com](https://www.makeareadme.com/) for this template.

## Suggestions for a good README

Every project is different, so consider which of these sections apply to yours. The sections used in the template are suggestions for most open source projects. Also keep in mind that while a README can be too long and detailed, too long is better than too short. If you think your README is too long, consider utilizing another form of documentation rather than cutting out information.

## Name
Choose a self-explaining name for your project.

## Description
Let people know what your project can do specifically. Provide context and add a link to any reference visitors might be unfamiliar with. A list of Features or a Background subsection can also be added here. If there are alternatives to your project, this is a good place to list differentiating factors.

## Badges
On some READMEs, you may see small images that convey metadata, such as whether or not all the tests are passing for the project. You can use Shields to add some to your README. Many services also have instructions for adding a badge.

## Visuals
Depending on what you are making, it can be a good idea to include screenshots or even a video (you'll frequently see GIFs rather than actual videos). Tools like ttygif can help, but check out Asciinema for a more sophisticated method.

## Installation
Within a particular ecosystem, there may be a common way of installing things, such as using Yarn, NuGet, or Homebrew. However, consider the possibility that whoever is reading your README is a novice and would like more guidance. Listing specific steps helps remove ambiguity and gets people to using your project as quickly as possible. If it only runs in a specific context like a particular programming language version or operating system or has dependencies that have to be installed manually, also add a Requirements subsection.

## Usage
Use examples liberally, and show the expected output if you can. It's helpful to have inline the smallest example of usage that you can demonstrate, while providing links to more sophisticated examples if they are too long to reasonably include in the README.

## Support
Tell people where they can go to for help. It can be any combination of an issue tracker, a chat room, an email address, etc.

## Roadmap
If you have ideas for releases in the future, it is a good idea to list them in the README.

## Contributing
State if you are open to contributions and what your requirements are for accepting them.

For people who want to make changes to your project, it's helpful to have some documentation on how to get started. Perhaps there is a script that they should run or some environment variables that they need to set. Make these steps explicit. These instructions could also be useful to your future self.

You can also document commands to lint the code or run tests. These steps help to ensure high code quality and reduce the likelihood that the changes inadvertently break something. Having instructions for running tests is especially helpful if it requires external setup, such as starting a Selenium server for testing in a browser.

## Authors and acknowledgment
Show your appreciation to those who have contributed to the project.

## License
For open source projects, say how it is licensed.

## Project status
If you have run out of energy or time for your project, put a note at the top of the README saying that development has slowed down or stopped completely. Someone may choose to fork your project or volunteer to step in as a maintainer or owner, allowing your project to keep going. You can also make an explicit request for maintainers.
>>>>>>> 2b459aaf137acd85bc15f1b50fa9ae40d3b5c8bb
