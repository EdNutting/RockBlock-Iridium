# RockBlock-Iridium

This library is an implementation of the AT command interface for RockBlock satellite devices connected to the Iridium network.

This repo also includes a demo application capable of sending and receiving messages to/from the Satellite Application Catapult network.

Please note, this library was implemented specifically for the 9602 Iridium Satellite chip mounted on a RockSeven RockBlock v2.B board, and as such may not support or work with other devices.


# How to use

Clone the source code, reference the project or build the DLL and reference it directly. Then follow one of the examples below to begin sending/receiving data.


# Physical configuration

Connect your RockBlock to a Windows computer using just the serial interface. You can either use a serial interface on your motherboard or (more commonly) use a USB2UART converter cable. 

Ensure your serial cable is recognised by checking in Device Manager - you will need to look for the *COM Port number* in Device Manager too.

# Examples

TODO