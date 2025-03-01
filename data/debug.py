# Debugging purposes only

import pandas as pd
import numpy
from astropy.coordinates import SkyCoord
import math
import csv

df = pd.read_csv(r"C:\Users\Ericb\Downloads\Many-Body-Dynamics-Barnes-Hut-main\Many-Body-Dynamics-Barnes-Hut-main\Main code\BarnesHut\BarnesHut\bin\Debug\animation.csv")
d = df.values

firstRow = d[3]
secondRow = d[4]
thirdRow = d[5]

print('blue' in d[2])

finalRowX = d[-4]
secondFinalRowX = d[-8]
finalRowY = d[-3]
secondFinalRowY = d[-7]
finalRowZ = d[-2]
secondFinalRowZ = d[-6]

posX2 = float(finalRowX[-1])
posY2 = float(finalRowY[-1])
posZ2 = float(finalRowZ[-1])

posX1 = float(secondFinalRowX[-1])
posY1 = float(secondFinalRowY[-1])
posZ1 = float(secondFinalRowZ[-1])

print(posX2 - posX1)

# Convert m/s and m to kpc/myrs and kpc
print("Position from center (x, y, z): ", posX2 - 0.001*(0.0005), posY2-0.0031*(0.0005), posZ2)
print("Final velocity in (x, y, z): ", ((posX2 - posX1))/0.0005, ((posY2 - posY1 - 0.0031)/2)/0.0005, ((posZ2 - posZ1)/20)/0.0005)
