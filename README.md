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
