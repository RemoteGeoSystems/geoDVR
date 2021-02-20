# geoDVR
|![RGS\_LogoColorTextBelow\_Crop](https://i.ibb.co/kBTSLRp/Aspose-Words-7da5ab13-f2ce-4e5f-8129-7a108bc82883-001.jpg)|<p>**geoDVR Ethernet Camera Target Location and Footprint Protocol** </p><p></p><p>*Last Updated: 2-19-21*</p>|<p>**Contact Information:**</p><p>3307 South College Avenue  </p><p>Suite 211</p><p>Fort Collins, CO 80525	</p>|
| :- | :-: | :- |


Version 1.3

Contact:

Web: [www.remotegeo.com](http://www.remotegeo.com)

Sales: <sales@remotegeo.com>

Support: <support@remotegeo.com>

Phone: +1 970-367-7808 

Copyright and Use Agreement

© Copyright 2019, Remote GeoSystems, Inc. All Rights reserved. The Remote GeoSystems name and

logo and all related product and service names, including geoDVR and LineVision, and design marks and slogans are the trademarks, and service marks of Remote GeoSystems, Inc.

Before loading, downloading, installing, upgrading or using any Licensed Product of Remote GeoSystems, Inc., users must read and agree to the license terms and conditions outlined in the Remote GeoSystems, Inc. 

[EndUser License Agreements](https://www.remotegeo.com/terms/) found at <https://www.remotegeo.com/terms/>.

All data, specifications, and information contained in this publication are based on information that we

believe is reliable at the time of printing. Remote GeoSystems, Inc. reserves the right to make changes

without prior notice.


Revision History

*Version 1.0 – 10-22-18*

*Version 1.1 – 10-30-18*

*Version 1.2 – 11-22-18*

*Version 1.3 – 3-5-19*

*Version 1.4 – 2-19-21*





1. **Overview**

The Remote GeoSystems, Inc’s geoDVR Ethernet Camera Target Location and Footprint Protocol defines the connectivity and data format requirements in order for the Remote Geo geoDVR to collect the required variables for displaying the camera target location and the camera footprint on the map when being used in the Remote Geo LineVision software suite.  Additionally, the protocol also allows the geoDVR Live Moving Maps module to display the camera target location along with the platform’s (aircraft, vehicle, etc) location.  Once collected and imported into LineVision, the data is presented in a format similar to the following:

![](https://i.ibb.co/0KdPSPk/Aspose-Words-7da5ab13-f2ce-4e5f-8129-7a108bc82883-002.png)

***IMPORTANT NOTE:*** The variables outlined in the Ethernet Camera Target Location and Footprint Protocol can also be captured and displayed in LineVision via the MISB STANAG protocol.  LineVision can handle both the MISB protocol and the protocol outlined in this document.  However, the purposed of this specification is to provide an alternate, greatly simplified protocol as compared to MISB that provides the minimum required variables for showing both the target location and camera footprint on the map.  *The coordinate system and order of operations used in the protocol outlined in this document parallels the ones used in the MISB specification.* 


2. **geoDVR Module Requirements**

The following geoDVR modules are required for the protocol to collect and display the required data:



1. Live Moving Maps module
2. Camera Target Footprint module *(Note: This has also previously been referred to in some documents as the Advanced Gimbal Targeting & 4-Corner MISB FMV module)*  

3. **Hardware & Software Requirements**

To implement the Ethernet Camera Target Location and Footprint Protocol outlined in this document, the device sending the data to the geoDVR must have an Ethernet port available that can send TCP data over a specified port.  By default, the geoDVR expects the data to be sent over port 1121.  This port can be changed via a configuration setting in the geoDVR, although it is recommended that the default port be used.  The device sending the information must have any firewalls, etc. configured to allow two-way communication over the default port.  The device sending the information must also have a static IP address that can be entered into the geoDVR.  It is recommended that the sending device be on the 192.168.0.1 subnet with a static IP address of 192.168.0.20.  The geoDVR automatically “listens” to this port/IP/subnet configuration although the configuration can be changed via a configuration setting in the geoDVR.

*In summary:*

Recommendations for devices implementing and sending protocol data:

- Recommended Port: 1121
- Recommended IP address: 192.168.0.20
- Recommended Subnet: 192.168.0.1
- Ethernet Protocol: TCP/IP

4. **Protocol Specification**

The data being sent via the protocol should be sent in constant intervals up to a maximum of 5 Hz.  Higher data frequencies are allowed although they have not been tested.  In general, the recommended frequency is 1 Hz.  The minimum recommended frequency is also 1 Hz.

The data being sent should be XML “packets” resembling the following: 

*(NOTE:  This example contains more than the minimum required fields.  The minimum required fields are outlined below)*

<trkpt>

`        `<lat>40.561952</lat> 

`        `<lon>-105.116984</lon>

`        `<ele>28.245</ele>

`        `<time>2018-11-22T13:45:30.0000000-07:00</time>

`        `<course>155.652455</course>

`        `<speed>1.734</speed>

`        `<fix>2</fix>

`        `<sat>21</sat>

`        `<hdop>0</hdop>

`        `<platform\_pitch>0.138795</platform\_pitch>

`        `<platform\_roll>-0.365271</platform\_roll>

`        `<platform\_heading>155.69548</platform\_heading>

`        `<sensor\_relative\_azimuth>15.65241</sensor\_relative\_azimuth>

`        `<sensor\_relative\_pitch>-39.603579</sensor\_relative\_pitch>

`        `<sensor\_relative\_roll>0</sensor\_relative\_roll>

`        `<slant\_range>0</slant\_range>

`        `<frame\_lat>40.56218547</frame\_lat>

`        `<frame\_lon>-105.11660662</frame\_lon>

`        `<frame\_ele>0.086115802687093082</frame\_ele>

`        `<horizontal\_fov>21.2300</horizontal\_fov>

</trkpt>

5. **Variable Definitions**


|**Variable**|**Description**|**Units**|
| :- | :- | :- |
|trkpt|This is the XML packet’s “root” element and must be included in all packets being sent.|N/A|
|lat|The latitude in decimal degree format.  At least six significant figures are recommended.|Decimal Degrees|
|lon|The longitude in decimal degree format.  At least six significant figures are recommended.|Decimal Degrees|
|ele|The altitude, in meters, above sea level.  At least three significant figures are recommended.|Meters above sea level|
|time|<p>GPS UTC Time in yyyy-MM-ddTHH:mm:ss.fffffffK format.</p><p></p><p>The format is the “Round-trip date/time pattern” as defined by “o” and “O” at <https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings>  </p>|<p>yyyy-MM-ddTHH:mm:ss.fffffffK</p><p>format with</p><p>“y” = year</p><p>“M” = month</p><p>“d” = day</p><p>“H” = 24 hour</p><p>“m” = minute</p><p>“s” = second</p><p>“f” = milliseconds</p><p>“K” = Time zone</p>|
|course|The course of the platform as true direction (relative to true north).  At lease six significant figures are recommended.|Degrees|
|speed|The speed of the platform in knots.  At least three significant figures are recommended.|Knots|
|fix|<p>GPS fix quality per the NMEA spec.</p><p></p><p>Fix quality: 0 = invalid</p><p>`                    `1 = GPS fix (SPS)</p><p>`                    `2 = DGPS fix</p><p>`                    `3 = PPS fix</p><p>`                    `4 = Real Time Kinematic</p><p>`	      `5 = Float RTK</p><p>`                    `6 = estimated (dead reckoning) (2.3 feature)</p><p>`	      `7 = Manual input mode</p><p>`	      `8 = Simulation mode</p>|Integer|
|sat|The number of satellites used in GPS calculations.|Integer|
|hdop|Horizontal dilution of precision.|Decimal number|
|platform\_pitch|See figure 1.  The platform pitch in degrees.  At least six significant figures are recommended. |Degrees|
|platform\_roll|See figure 1.  The platform roll in degrees.  At least six significant figures are recommended.|Degrees|
|platform\_heading|See figure 1.  The platform heading in degrees as true direction.  At least six significant figures are recommended.|Degrees|
|sensor\_relative\_azimuth|See figure 1.  The sensor (camera direction) azimuth relative to the platform.  At least six significant figures are recommended.|Degrees|
|sensor\_relative\_pitch|See figure 1.  The sensor (camera) pitch relative to the platform.  At least six significant figures are recommended.|Degrees|
|sensor\_relative\_roll|The sensor (camera) roll relative to the platform.  At least six significant figures are recommended.|Degrees|
|slant\_range|The distance between the sensor and the ground intersection.  At least three significant figures are recommended.|Meters|
|frame\_lat|The target location latitude as decimal degrees.  At least six significant figures are recommended.|Decimal Degrees|
|frame\_lon|The target location longitude as decimal degrees.  At least six significant figures are recommended.|Decimal Degrees|
|frame\_ele|The target altitude, in meters, above sea level.  At least three significant figures are recommended.|Meters above sea level|
|horizontal\_fov|The horizontal field-of-view of the camera(s).  At least six significant figures are recommended.|Degrees|

***IMPORTANT: The angles and variables mentioned above MUST follow the coordinate system defined in the MISB specification.  Please refer to Figure 1 below.***

![picture3](https://i.ibb.co/cL7h2D1/Aspose-Words-7da5ab13-f2ce-4e5f-8129-7a108bc82883-003.png)![](https://i.ibb.co/52Cw1hF/Aspose-Words-7da5ab13-f2ce-4e5f-8129-7a108bc82883-004.png)

**Figure 1**


6. **Minimum Required Variables**

If frame center (sensor target location) calculations are present, the minimum variables required are the following:

<trkpt>

`        `<lat>40.561952</lat> 

`        `<lon>-105.116984</lon>

`        `<ele>28.245</ele>

`        `<time>2018-11-22T13:45:30.0000000-07:00</time>

`        `<course>155.652455</course>

`        `<speed>1.734</speed>

`        `<frame\_lat>40.56218547</frame\_lat>

`        `<frame\_lon>-105.11660662</frame\_lon>

`        `<frame\_ele>0.0861158026870</frame\_ele>

`        `<horizontal\_fov>21.2300</horizontal\_fov>

</trkpt>

Provided the frame center calculations are present, at a minimum the horizontal field-of-view variable (hfov1) from one camera (typically the TV camera) needs to be provided along with the “standard” platform GPS data.  Using this data, LineVision will dynamically calculate the vertical field-of-view based on the recorded video’s aspect ratio.

7. **geoDVR Configuration** 

To configure the geoDVR to receive GPS data from your gimbal/device, navigate to the File Menu and select “Options->Use Remote Geo Ethernet GPS” from the drop-down menu. This setting configures the geoDVR to listen for incoming connections (it behaves like a server) and the "ANY" address resolves to 0.0.0.0 which allows connections from any client.

![](https://i.ibb.co/RCjY4q0/Aspose-Words-7da5ab13-f2ce-4e5f-8129-7a108bc82883-005.png)


8. **Camera Control**

The geoDVR can be used to control your gimbal.  If enabled, they the map is touched (clicked), the location touched is sent over a network connection.  The data packet containing the location coordinates where the map was “touched” takes on the form:

<GPStouch> 

<lat></lat> 

<lon></lon> 

</GPStouch>

In the above, <lat> is a numeric value representing the latitude and <lon> is a numeric value representing the longitude.

To receive the coordinates, two methods are available.  The first option is by using the existing TCP Ethernet connection that is sending the GPS/frame center coordinates to the geoDVR (as configured above when “Options->Use Remote Geo Ethernet GPS” is selected from the main menu).  When the coordinates are sent to the geoDVR, a two-way TCP connection is opened and whenever the map is touched (clicked), the above xml packet is passed back over the connection and the specified port.



The second option is by listening to a UDP Connection that is broadcast from the geoDVR.  The UDP packets also take on the same format as above and are configured when “Moving Maps->Enable Camera Control” is selected from the main menu (IMPORTANT: “Enable Camera Control” must be selected for camera control coordinates to be sent):

![](https://i.ibb.co/XL4gpfm/Aspose-Words-7da5ab13-f2ce-4e5f-8129-7a108bc82883-006.png)

![](https://i.ibb.co/hWzJ7kz/Aspose-Words-7da5ab13-f2ce-4e5f-8129-7a108bc82883-007.png
)

In this case, the IP Address specified must be the IP address of the gimbal/computer where the camera control coordinates are being sent.

Once the above are configured, a button towards the top of the map labeled “Enable Camera Positioning on Click” should appear.

![](https://i.ibb.co/zP2Rfzj/Aspose-Words-7da5ab13-f2ce-4e5f-8129-7a108bc82883-008.png
)


When selected, camera control coordinates will be sent over both the TCP and UDP connections.  IMPORTANT: Only one connection, either TCP or UDP, is required for gimbal control.  The two different options are offered to give more flexibility in programming options.

When touched (clicked), the text should change to “Disable Camera Positioning on Click” and the button will be highlighted yellow.



![](https://i.ibb.co/C8L26vV/Aspose-Words-7da5ab13-f2ce-4e5f-8129-7a108bc82883-009.png) 

When selected, each time you touch the map, it will send a data point on that exact spot.

9. **Protocol Test Application, Example Source Code**

To assist with sending GPS to the geoDVR and to test Camera Control connection, source code has been placed on the Remote GeoSystems GitHub account.  A test application can be downloaded from:

<https://github.com/RemoteGeoSystems/geoDVR/tree/geoDVR/RemoteGeoProtocolXmlTestApp/RemoteGeoProtocolXmlTestApp/bin/Release> 

The source code for the application can be downloaded from the same repository at:

<https://github.com/RemoteGeoSystems/geoDVR/tree/geoDVR/RemoteGeoProtocolXmlTestApp> 

To run the source code, Visual Studio 2019 is recommended.  The source code is provided in C#.

10. **Questions and Additional Support**

For questions or additional support, please contact the support email address listed above or visit the Remote GeoSystems, Inc. website at [www.remotegeo.com](http://www.remotegeo.com) or visit the Remote Geo Product Support forums at <https://support.remotegeo.com/forums/>.  

The geoDVR webpage is located at <https://www.remotegeo.com/geodvr/>. 

LineVision information can be found at <https://www.remotegeo.com/software/linevision-desktop/>. 
