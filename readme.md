# Graph Navigation with Eye-tracker
Creating a gaze-aided graph navigating application using [Unity3D](https://unity3d.com/) and [Pupil-lab](https://github.com/pupil-labs/pupil).

### Features:
* [Dataset](https://snap.stanford.edu/data/#wikipedia) from wikipedia is sampled and projected in 3D space using [Force-directed-layouts](https://en.wikipedia.org/wiki/Force-directed_graph_drawing).
    * Inspired by this [post](http://collaboradev.com/2014/03/12/visualizing-3d-network-topologies-using-unity/).
* Supports keyboard/mouse controls.
* Navigation/Rotation and Dramatic Camera in Node selection.
* ~~Information panel added for each selected Node to render extra information.~~
* Bubble technique to narrow down selection area for user[1].

### Why?
* Part of the masters project supervised by the [Human-Computer-Interaction Group, Hamburg University](https://www.inf.uni-hamburg.de/en/inst/ab/hci.html).

### TODO:
* ~~Implementation of Dwell Time.~~
* ~~Integrate HapRing[2] as a control device.~~
* ~~Enhance the eye tracking with regression/interpolation[3]~~. 

### Collaborators
* [Ahmed Elsafty](https://github.com/Saftophobia)
* [Mirco Franzek](https://github.com/ablx)
* [Christian Peter](https://github.com/ChristianPe)
* [Tim Dobert](https://github.com/Taldops)
* [Savitha Nagaraju](https://github.com/SavithaNagaraju)
* Verena Metz

### Samples
###### Graph View
![Sample](https://raw.githubusercontent.com/Saftophobia/graph-viz-eye-tracker/master/util/readme/scrnsht_05_oct.png)

### References:
[1] Dominjon, Lionel, et al. "The" Bubble" technique: interacting with large virtual environments using haptic devices with limited workspace." *Eurohaptics Conference, 2005 and Symposium on Haptic Interfaces for Virtual Environment and Teleoperator Systems, 2005. World Haptics 2005. First Joint. IEEE, 2005*.

[2] Nunez, Oscar Javier Ariza, Paul Lubos, and Frank Steinicke. "HapRing: A Wearable Haptic Device for 3D Interaction." *Mensch & Computer. 2015*.

[3] Scheel, Christian, A. B. M. Islam, and Oliver Staadt. "An Efficient Interpolation Approach for Low Cost Unrestrained Gaze Tracking in 3D Space." (2016).
