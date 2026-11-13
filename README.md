# Teleoperated_Torso

### Brief Description

Robotic torso controlled wirelessly via a vr headset

### Problem Statement / Goal

My inspiration for this project comes from space. Astronauts aboard the ISS need to do spacewalks to complete required maintenance and updates to the station. There are however a couple problems with space walks, they are very physically demanding, take a long time, and increase the astronauts exposure to radiation. This goal of this project is to make a robot that can complete the space walk while being controlled by an astronaut inside the station, or from ground control. 

### Design Overview

The design of this robotic teleoperated torso (RTT) consists of two arms and a head. The robot is controlled using a stand alone vr headset and is designed to mimic the movements of the person using the headset. For example, if the user extends their right arm out in front of themselves, the robot should, with minimal latency, also extend its right arm out in front of itself.

### Frame

The frame for the robot was designed entirely in Onshape and 3D printed using PLA. There are 6 parts: The Body, Camera Mount, Left Bicep, Right Bicep, Left Forearm, and Right Forearm. The proportions for the design match my personal proportions scaled by a factor of 0.4. Picture of the frame are shown below

Picture of the frame

### Electronics

#### List of Components:

- Arduino R4 Wifi
- Buck Converter (8-40V input, 12V 6A output)
- Bus Servo Adapter
- Li-Po Battery (3300mAh, 50C, 14.8V)
- PCA9685 Servo Driver
- Serial Bus Servo x2
- UBEC (8A, 15A peak)
- 20kg Digital Servo x6
- XT60 Switch

#### System Block Diagram

<img width="793" height="463" alt="PowerMapRTT drawio" src="https://github.com/user-attachments/assets/b3517c0d-730b-436d-a28d-b4f5ddee1a53" />
