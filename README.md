[![](https://jitpack.io/v/texttechnologylab/VR-Parliamentary-Debates-Explorer.svg)](https://jitpack.io/#texttechnologylab/VR-Parliamentary-Debates-Explorer)
![GitHub License](https://img.shields.io/github/license/Texttechnologylab/VR-Parliamentary-Debates-Explorer)

[![Conference](http://img.shields.io/badge/conference-Hypertext--2025-4b44ce.svg)](https://ht.acm.org/ht2025/)
[![Paper](http://img.shields.io/badge/paper-Hypertext--2025-4b44ce.svg)](https://doi.org/10.1145/3720553.3746672)


# VR-Parliamentary Debates Explorer
VR-ParlExplorer: A Hypertext System for the Collaborative Interaction in Parliamentary Debate Spaces

[<img src="https://img.youtube.com/vi/XJTRjPCPtuE/hqdefault.jpg" width="500" height="300"
/>](https://www.youtube.com/embed/XJTRjPCPtuE)

# Abstract
VR-ParlExplorer is implemented as a extension for [Va.Si.Li-Lab](https://github.com/texttechnologylab/Va.Si.Li-Lab) to enable immersion in the dynamics of communication in parliamentary debates. The system virtualizes plenary debates that allows users to interact with virtual members of parliament through chatbots powered by Large Language Models. VR-ParlExplorer facilitates automatic processing, presentation and interaction with plenary debates using Natural Language Processing, through the [Docker Unified UIMA Interface](https://github.com/texttechnologylab/DockerUnifiedUIMAInterface), and speech technologies, while extending the functionality of Va.Si.Li-Lab. All speeches, grouped by session and agenda items, can be replayed individually using an avatar of the speaker and text-to-speech software. Users can interact with speaker avatars using fine-tuned language models based on Members of Parliament speeches, realized through TTS and speech-to-text models.

### How to start
1. Follow Va.Si.Li-Lab's [Setup Guide](https://texttechnologylab.github.io/Va.Si.Li-Lab/getting_started/setting_up/#clone-the-repository)
2. Add the "Bundestag" Scene to the database or start it directly
3. (Optional) Load speeches into the [Backend](https://github.com/texttechnologylab/VR-Parliamentary-Debates-Explorer/tree/main/Backend)
4. Start the [Backend](https://github.com/texttechnologylab/VR-Parliamentary-Debates-Explorer/tree/main/Backend)
5. Adjust urls in the Unity scene under the "Speech selector Panel" and "MataPlayer" -> "VoiceListener"
6. Click play

# Team
- [Daniel Bundan](https://github.com/Mocretion)
- Chrisowaladis Manolis
- [Giuseppe Abrami](https://github.com/abrami) (Supervision)
- [Prof. Dr. Alexander Mehler](https://www.texttechnologylab.org/team/alexander-mehler/) (Supervision)

# BibTeX
```
@inproceedings{Abrami:et:al:2025:c,
  author    = {Abrami, Giuseppe and Bundan, Daniel and Manolis, Chrisowaladis
               and Mehler, Alexander},
  title     = {VR-ParlExplorer: A Hypertext System for the Collaborative Interaction
               in Parliamentary Debate Spaces},
  year      = {2025},
  isbn      = {9798400715341},
  publisher = {Association for Computing Machinery},
  address   = {New York, NY, USA},
  url       = {https://doi.org/10.1145/3720553.3746672%7D,
  doi       = {10.1145/3720553.3746672},
  abstract  = {The enhanced visualization and interaction with information in
               collaborative VR environments enabled by chatbots is currently
               rather limited. To fill this gap and create a concrete application
               that combines spatial and virtual concepts of hypertext systems
               based on the use of LLMs, we present VR-ParlExplorer as a system
               for virtualizing plenary debates that allows users to interact
               with virtual members of parliament through chatbots. VR-ParlExplorer
               is implemented as a Plugin for Va.Si.Li-Lab to enable immersion
               in the dynamics of communication in parliamentary debates. The
               paper describes the functionality of VR-ParlExplorer and discusses
               specifics of the use case it addresses.},
  booktitle = {Proceedings of the 36th ACM Conference on Hypertext and Social Media},
  pages     = {177--183},
  numpages  = {7},
  location  = {Chicago, USA},
  series    = {HT '25},
}
```
