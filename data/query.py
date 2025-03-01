# Get all data required to compute the velocity and mass of each star.
# For calculation details, check the post attached to the Github repo.
# For the batch computations, check the star_calculations.py file.
# If you are planning on running this query yourself, astroquery is required.
# WARNING: Doing a query of this size took me quite a long time. These SQL commands are mostly for demo purposes.

from astroquery.gaia import Gaia
from astropy.coordinates import SkyCoord
from astropy import units as u

query = f'''
    SELECT solution_id, ra, dec, parallax, pmra, pmdec, parallax, phot_g_mean_flux, radial_velocity, logg_gspphot, teff_gspphot, pm
    FROM gaiadr3.gaia_source
    WHERE DISTANCE(18, -28, ra, dec) < 4900./60
    AND 1/(0.001*parallax) <= 12000
    AND 1/(0.001*parallax) >= 8000
    AND parallax IS NOT NULL
    AND phot_g_mean_flux IS NOT NULL
    AND radial_velocity IS NOT NULL
    AND logg_gspphot IS NOT NULL
    AND teff_gspphot IS NOT NULL
    AND pm IS NOT NULL
'''

# Perform the query
job = Gaia.launch_job_async(query)
results = job.get_results()

results.write('results3.csv', format='csv', overwrite=True)
