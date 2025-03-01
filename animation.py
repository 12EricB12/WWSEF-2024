import matplotlib.pyplot as plt
import matplotlib.animation as animation
from matplotlib import rcParams

import InputOutput as IO

# array = IO.ReadCSV(r"C:\Users\Ericb\Desktop\Folderception\Final Simulation Results\120000.csv")
array = IO.ReadCSV(r"C:\Users\Ericb\Downloads\Many-Body-Dynamics-Barnes-Hut-main\Many-Body-Dynamics-Barnes-Hut-main\Main code\BarnesHut\BarnesHut\bin\Debug\animation.csv")

xarray = array[0::4]
yarray = array[1::4]
zarray = array[2::4]
colarray = array[3::4]

rcParams['animation.ffmpeg_path'] = r"C:\ffmpeg\ffmpeg-2022-04-14-git-ea84eb2db1-full_build\bin\ffmpeg.exe"

arrayA = xarray
arrayB = yarray
arrayC = zarray

a = 4

fig = plt.figure()
ax = fig.add_subplot(projection='3d')

ax.set(xlim=(-a, a), ylim=(-a, a), zlim=(-a, a))
ax.set_facecolor('black')
ax.tick_params(axis='x', colors='white')
ax.tick_params(axis='y', colors='white')
ax.tick_params(axis='z', colors='white')

sc = ax.scatter(arrayA[0], arrayB[0], arrayC[0], c=colarray[0], marker='o', s=1)

print('white' in colarray[0])

def update(frame):
    sc._offsets3d = (arrayA[frame], arrayB[frame], arrayC[frame])
    sc.set_color(colarray[frame])
    plt.title("frame " + str(frame), fontsize=18)


ani = animation.FuncAnimation(fig, update,
                              frames=len(xarray) - 1, interval=50, repeat=True)

plt.show()

plt.plot(arrayA, arrayB, arrayC)
plt.savefig("fig.pdf")

Writer = animation.writers['ffmpeg']
writer = Writer(fps=60, metadata=dict(artist='Me'), bitrate=1800)

ani.save('adf.mp4', writer=writer)
