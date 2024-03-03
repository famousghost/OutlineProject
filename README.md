# Outline Shader HDRP
## Unity Version: 2022.3.18f
## HDRP Version: 14.0.10
## MC_OutlineObject:
- Attach this script to work with outline object feature
- Adjust parameters as you want
## Custom pass volume "MC_OutlineCustomPass"
- Add this custom pass volume in order to inform HDRP to render objects outline
## MC_OutlineManager
- Add this manager in order to inform Custom pass how many objects has MC_OutlineObject scripts
- Choose Settings from predefined or add your own (Settings gives you possibility to change stencil buffer shader and outline shader as you want)

After all these steps you can see this result with different objects simple and advanced like vegetation
![image](https://github.com/famousghost/OutlineProject/assets/23434168/b6011479-c0d2-4a73-a9de-c1aff65cb746)

## Troubleshooting
- If there is an issue with displaying these objects, try disabling and then enabling the MC_OutlineManager.
