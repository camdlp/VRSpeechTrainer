# VRSpeech

VRSpeech is a virtual reality (VR) application that lets you practice your presentations. It also includes a Flask API, located in the `./API` directory, which handles the backend functionality for the application.

## Getting Started

To get started with VRSpeech, follow the steps below:

### Prerequisites

- Unity 2021.3 or later
- Oculus Integration package for Unity
- Oculus Link cable
- Oculus compatible VR headset
- Python 3.6 or later

### Installation

1. Clone the repository:
`git clone https://github.com/yourusername/VRSpeech.git`
2. Open the Unity project by launching Unity and selecting the `VRSpeech` folder.

3. Install the Oculus Integration package from the Unity Asset Store.

4. Set up the Oculus Link cable and connect your Oculus compatible VR headset to your PC.

5. Install the Flask API dependencies:
`cd VRSpeech/API`
`pip install -r requirements.txt`

6. Run the Flask API:
`python app.py`

### Running VRSpeech

1. In Unity, go to `File > Build Settings`, select your target platform, and click `Switch Platform`.

2. Click `Add Open Scenes` to add the current scene to the build.

3. If it doesn't work and the app is not compiled, you will need to run it using Oculus Link. Make sure your Oculus headset is connected to your PC via the Oculus Link cable.

4. Press the `Play` button in the Unity Editor to run the application.

## User Guide

This user guide will help you get familiar with the VRSpeech application and its features.

### Navigating the VR Environment

1. Put on your Oculus-compatible VR headset and make sure it is connected to your PC via the Oculus Link cable.

2. Use the VR controllers to move around the virtual environment and interact with objects.

3. The application may include various buttons, menus, or interactive elements that you can select or manipulate using your VR controllers.

### Using the Flask API

The Flask API provides the backend functionality for VRSpeech. It runs on your local machine and communicates with the Unity application. To use the API, follow these steps:

1. Make sure the Flask API is running. If not, start it by following the instructions in the [Installation](#installation) section.

2. While using the VR application, any requests made to the API will be automatically handled by the Flask server.

3. If needed, you can monitor the API logs for any errors, warnings, or status updates. The logs will be printed to the console where the API is running.

## Troubleshooting

If you encounter any issues while using VRSpeech, try the following troubleshooting steps:

1. Make sure your VR headset is connected to your PC via the Oculus Link cable, and the Oculus software is running on your PC.

2. Ensure the Flask API is running and all dependencies are installed correctly.

3. Check the Unity console for any error messages or warnings.

4. Consult the [Unity documentation](https://docs.unity3d.com/Manual/index.html) and [Oculus documentation](https://developer.oculus.com/documentation/) for additional support.

## Contributing

If you would like to contribute to the VRSpeech project, please follow these steps:

1. Fork the repository on GitHub.

2. Create a new branch for your feature or bugfix.

3. Make your changes and commit them to your branch.

4. Submit a pull request, and provide a detailed description of your changes.

## License
VRSpeech is licensed under the [GLPv3](https://www.gnu.org/licenses/gpl-3.0.html).

## Acknowledgements
[Unity](https://unity.com/)
[Oculus (Meta)](https://www.meta.com/quest/)
[Oculus Link](https://www.meta.com/help/quest/articles/headsets-and-accessories/oculus-link/connect-link-with-quest-2/)
[Flask](https://flask.palletsprojects.com/en/2.0.x/)