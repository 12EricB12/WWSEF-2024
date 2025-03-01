# All the batch calculations.
# Read the post for more detail/justification for each step.

import pandas as pd
import numpy
from astropy.coordinates import SkyCoord
import math
import csv

df = pd.read_csv("results3.csv")
d = df.values

finalData = []

for i in range(len(d)):
    data = d[i]

    # CONSTANTS
    sunCurrLong = 272
    Vlsr = 184

    VlsrX = -9.35
    VlsrRad = Vlsr*math.sin(sunCurrLong)
    VlsrY = Vlsr*math.cos(sunCurrLong)

    VSunLSRx = 11.1 - 0.75 - 1
    VSunLSRy = 4 + 0.47
    VSunLSRz = 6.0 - 0.36 - 0.5

    VSunLSRx *= 0.123
    VSunLSRy *= 0.123
    VSunLSRz *= 0.123

    dSun = 8

    eV = 1.60218e-19

    # Find velocity
    solID = data[0]*pow(10, -3)

    pmdec = data[5]
    pmra = data[4]
    vRad = 0.123*data[8]

    theta = math.atan(pmdec/pmra)
    pm = math.sqrt(pow(pmdec, 2) + pow(pmra, 2))
    dist = 1/(0.001*data[3]) # in as, pc

    vT = 4.744e-3*dist*pm
    vT *= 0.123
    vX, vY, vZ = 0, 0, 0

    if vRad > 0:
        vX = 0.1*(vRad + VSunLSRx + abs(VlsrRad))
    else:
        vX = -0.1*((vRad + VSunLSRx) + abs(VlsrRad))

    vY = (vT*math.sin(theta))-VSunLSRy-VlsrY+2.2
    vZ = (vT*math.cos(theta))-VSunLSRz

    # find r
    rX, rY, rZ = 0, 0, 0

    ra = data[1]
    dec = data[2]

    c = SkyCoord(ra=ra, dec=dec, frame='icrs', unit='deg')

    cGalactic = c.galactic

    l = cGalactic.l.degree
    b = cGalactic.b.degree

    dist /= 1000

    if dist > 8:
        rX = abs(dist*math.cos(b)*math.cos(l))-8
    else:
        rX = -(8-abs(dist*math.cos(b)*math.cos(l)))

    rY = dist*math.cos(b)*math.sin(l)
    rZ = dist*math.sin(b)

    # find mass
    F = data[7]
    g = pow(10, data[9])*100
    Teff = data[10]

    G = 6.6743e-11
    stefBoltzConst = 5.6703e-8

    L = 4*math.pi*(pow(3.08567758e16*dist, 2))*(1.60218e-19*F)
    m = (g*L)/(4*math.pi*stefBoltzConst*pow(Teff, 4)*G)

    m *= 10e-36

    values = [solID, rX, rY, rZ, vX, vY, vZ, m]
    finalData.append(values)

with open("masses.csv", "w+", newline='') as mycsv:
    csvWriter = csv.writer(mycsv, delimiter=',')
    csvWriter.writerows(finalData)

print("Done")