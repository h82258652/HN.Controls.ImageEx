# HN.Controls.ImageEx
[![Build status](https://github.com/h82258652/HN.Controls.ImageEx/workflows/CI/badge.svg)](https://github.com/h82258652/HN.Controls.ImageEx/workflows/CI/badge.svg)

| Package                  | Nuget                                                                                                                            |
| -                        | -                                                                                                                                |
| HN.Controls.ImageEx.Core | [![Nuget](https://img.shields.io/nuget/v/HN.Controls.ImageEx.Core.svg)](https://www.nuget.org/packages/HN.Controls.ImageEx.Core) |
| HN.Controls.ImageEx.Wpf  | [![Nuget](https://img.shields.io/nuget/v/HN.Controls.ImageEx.Wpf.svg)](https://www.nuget.org/packages/HN.Controls.ImageEx.Wpf)   |
| HN.Controls.ImageEx.Uwp  | [![Nuget](https://img.shields.io/nuget/v/HN.Controls.ImageEx.Uwp.svg)](https://www.nuget.org/packages/HN.Controls.ImageEx.Uwp)   |

An enhanced image for WPF and UWP.  
WPF version for .net framework 4.6.2 and .net core 3.1  
UWP version for 17763  
The usage please see the demo project.  

### WPF Support
|                   | ImageEx            | ImageBrushEx       |
| -                 | -                  | -                  |
| Disk Cache        | :heavy_check_mark: | :heavy_check_mark: |
| Placeholder       | :heavy_check_mark: |                    |
| Loading Template  | :heavy_check_mark: |                    |
| Failed Template   | :heavy_check_mark: |                    |
| Retry             | :heavy_check_mark: | :heavy_check_mark: |
| Lazy Loading      | :heavy_check_mark: |                    |
| CornerRadius      | :heavy_check_mark: |                    |
| Shadow            | :heavy_check_mark: |                    |
| Gif               | :heavy_check_mark: |                    |
| Webp              | :heavy_check_mark: |                    |
| Fade in animation | :heavy_check_mark: |                    |

### UWP Support
|                   | ImageEx            | ImageBrushEx       |
| -                 | -                  | -                  |
| Disk Cache        | :heavy_check_mark: | :heavy_check_mark: |
| Placeholder       | :heavy_check_mark: |                    |
| Loading Template  | :heavy_check_mark: |                    |
| Failed Template   | :heavy_check_mark: |                    |
| Retry             | :heavy_check_mark: | :heavy_check_mark: |
| Lazy Loading      | :heavy_check_mark: |                    |
| CornerRadius      | :heavy_check_mark: |                    |
| Shadow            | :heavy_check_mark: |                    |
| Gif               | :heavy_check_mark: |                    |
| Webp              | :heavy_check_mark: | :heavy_check_mark: |
| Fade in animation | :heavy_check_mark: |                    |

### Next plan: 
- [x] Update the lazy loading, support set the thresholds and give a default value. (Now is 300px)  
- [x] Built-in fade in animation while loaded image success. (Default duration is 0 second)  
- [x] Use ```SkiaSharp``` to resolve the image.  
- [x] ```CornerRadius``` support.  
- [x] Built-in shadow, same API for WPF and UWP.  
- [x] Gif animation controller, including ```Play```, ```Pause```, ```GotoFrame```, ```RepeatBehavior```(use metadata or custom), ```SpeedRatio```.  
- [x] Same API for WPF and UWP, such as ```StretchDirection```(exist in WPF but not UWP), ```NineGrid```(exist in UWP but not WPF).  
- [ ] Add ```IImageEx``` interface for ```ImageEx```.  
- [ ] Improve performance.  
- [ ] Target .net 5.  
