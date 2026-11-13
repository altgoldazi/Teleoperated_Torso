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

- Arduino UNO R4 Wifi
- Buck Converter (8-40V input, 12V 6A output)
- Bus Servo Adapter
- Li-Po Battery (3300mAh, 50C, 14.8V)
- PCA9685 Servo Driver
- Serial Bus Servo x2
- UBEC (8A, 15A peak)
- 20kg Digital Servo x6
- XT60 Switch

#### Power System Overview

The main power supply is a 14.8V Li-Po battery. The 14.8V line runs directly through a heavy duty XT60 switch to allow for easy toggling of the power system. It is then split creating to sides, a lower voltage 6V side and a higher voltage 12V side. This is to correctly power the different components with the needed voltages. For example, the digital servos require 6V to run while the serial bus servos require 12V.

12V Side: A buck converter is used to step down the voltage from 14.8V to 12V. Then the output is split to power the bus servo adapter and the Arduino UNO R4 Wifi.
6V Side: A UBEC is used to lower the voltage from 14.8V to 6V. The 6V is then wired to the screw terminal of the servo driver board and is used to power the servos.
5V line: The Arduino has internal components that lower the input voltage from 12V to 5V. The 5V output from the arduino is connected to the servo driver to power the boards logic.

#### System Block Diagram

<img width="859" height="649" alt="image" src="https://github.com/user-attachments/assets/fc58b372-1e4a-4b43-8841-bbb85a933bf3" />

