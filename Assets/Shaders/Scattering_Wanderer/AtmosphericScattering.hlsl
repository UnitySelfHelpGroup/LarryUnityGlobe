bool rayIntersectsSphere(
    float3 rayStart, float3 rayDir, float3 sphereCenter, float sphereRadius, out float t0, out float t1) {
    float3 oc = rayStart - sphereCenter;
    float a = dot(rayDir, rayDir);
    float b = 2.0 * dot(oc, rayDir);
    float c = dot(oc, oc) - sphereRadius * sphereRadius;
    float d = b * b - 4.0 * a * c;

    // Also skip single point of contact
    if (d <= 0.0) {
        return false;
    }

    float r0 = (-b - sqrt(d)) / (2.0 * a);
    float r1 = (-b + sqrt(d)) / (2.0 * a);

    t0 = r0;
    t1 = r1;
    //t0 = min(r0, r1);
    //t1 = max(r0, r1);

    //return (t1 >= 0.0);
    return true;
}

float4 AtmosphericScattering(
    float3 start, 				// the start of the ray (the camera position)
    float3 dir, 					// the direction of the ray (the camera vector)
    float max_dist, 			// the maximum distance the ray can travel (because something is in the way, like an object)
    float3 scene_color,			// the color of the scene
    float3 light_dir, 			// the direction of the light
    float3 light_intensity,		// how bright the light is, affects the brightness of the atmosphere
    float3 planet_position, 		// the position of the planet
    float planet_radius, 		// the radius of the planet
    float atmo_radius, 			// the radius of the atmosphere
    float3 beta_ray, 				// the amount rayleigh scattering scatters the colors (for earth: causes the blue atmosphere)
    float3 beta_mie, 				// the amount mie scattering scatters colors
    float3 beta_absorption,   	// how much air is absorbed
    float3 beta_ambient,			// the amount of scattering that always occurs, cna help make the back side of the atmosphere a bit brighter
    float g, 					// the direction mie scatters the light in (like a cone). closer to -1 means more towards a single direction
    float height_ray, 			// how high do you have to go before there is no rayleigh scattering?
    float height_mie, 			// the same, but for mie
    float height_absorption,	// the height at which the most absorption happens
    float absorption_falloff,	// how fast the absorption falls off from the absorption height
    int steps_i, 				// the amount of steps along the 'primary' ray, more looks better but slower
    int steps_l, 				// the amount of steps along the light ray, more looks better but slower
    float atmosphereIntensity,
    float scatteringIntensity,
    float4 atmosphereColor
) {
    float totalRadius = planet_radius + (atmo_radius * atmosphereIntensity);
    height_ray *= atmosphereIntensity;
    height_mie *= atmosphereIntensity;
    beta_ray *= scatteringIntensity;
    beta_mie *= scatteringIntensity;
    beta_absorption *= scatteringIntensity;
    beta_ambient *= scatteringIntensity;

    // add an offset to the camera position, so that the atmosphere is in the correct position
    start -= planet_position;

    float t0, t1;
    bool intersection = rayIntersectsSphere(start, dir, float3(0,0,0), totalRadius, t0, t1);

    // stop early if there is no intersect
    if (!intersection) return float4(0,0,0, 0);

    float2 ray_length = float2(max(0, t0), min(max_dist, t1));

    // if the ray did not hit the atmosphere, return a black color
    if (ray_length.x > ray_length.y) return float4(0,0,0,0);
    // prevent the mie glow from appearing if there's an object in front of the camera
    bool allow_mie = max_dist > ray_length.y;
    // make sure the ray is no longer than allowed
    ray_length.y = min(ray_length.y, max_dist);
    ray_length.x = max(ray_length.x, 0.0);
    // get the step size of the ray

    float step_size_i = (ray_length.y - ray_length.x) / float(steps_i);
    
    // next, set how far we are along the ray, so we can calculate the position of the sample
    // if the camera is outside the atmosphere, the ray should start at the edge of the atmosphere
    // if it's inside, it should start at the position of the camera
    // the min statement makes sure of that
    float ray_pos_i = ray_length.x;

    // these are the values we use to gather all the scattered light
    float3 total_ray = float3(0, 0, 0); // for rayleigh
    float3 total_mie = float3(0, 0, 0); // for mie
    
    // initialize the optical depth. This is used to calculate how much air was in the ray
    float3 opt_i = float3(0,0,0);
    
    // also init the scale height, avoids some float2's later on
    float2 scale_height = float2(height_ray, height_mie);
    
    // Calculate the Rayleigh and Mie phases.
    // This is the color that will be scattered for this ray
    // mu, mumu and gg are used quite a lot in the calculation, so to speed it up, precalculate them
    float mu = dot(dir, light_dir);
    float mumu = mu * mu;
    float gg = g * g;
    float phase_ray = 3.0 / (16 * PI) * (1.0 + mumu);
    float phase_mie = allow_mie ? 3.0 / (8 * PI) * ((1.0 - gg) * (mumu + 1.0)) / (pow(1.0 + gg - 2.0 * mu * g, 1.5) * (2.0 + gg)) : 0.0;
    
    // now we need to sample the 'primary' ray. this ray gathers the light that gets scattered onto it
    for (int i = 0; i < steps_i; ++i) {


        // calculate where we are along this ray
        float3 pos_i = start + dir * (ray_pos_i + step_size_i * 0.5);

        // and how high we are above the surface
        float height_i = length(pos_i) - planet_radius;

        // now calculate the density of the particles (both for rayleigh and mie)
        float3 density = float3(exp(-height_i / scale_height), 0.0);

        // and the absorption density. this is for ozone, which scales together with the rayleigh, 
        // but absorbs the most at a specific height, so use the sech function for a nice curve falloff for this height
        // clamp it to avoid it going out of bounds. This prevents weird black spheres on the night side
        density.z = clamp((1.0 / cosh((height_absorption - height_i) / absorption_falloff)) * density.x, 0.0, 1.0);
        density *= step_size_i;

        // Add these densities to the optical depth, so that we know how many particles are on this ray.
        opt_i += density;

        // Calculate the step size of the light ray.
        // again with a ray sphere intersect
        // a, b, c and d are already defined
        rayIntersectsSphere(pos_i, light_dir, float3(0, 0, 0), totalRadius, t0, t1);
        float step_size_l = (t1 - t0) / float(steps_l);

        // and the position along this ray
        // this time we are sure the ray is in the atmosphere, so set it to 0
        float ray_pos_l = 0.0;

        // and the optical depth of this ray
        float3 opt_l = float3(0,0,0);
        // now sample the light ray
        // this is similar to what we did before
        for (int l = 0; l < steps_l; ++l) {

            // calculate where we are along this ray
            float3 pos_l = pos_i + light_dir * (ray_pos_l + step_size_l * 0.5);

            // the heigth of the position
            float height_l = length(pos_l) - planet_radius;

            // calculate the particle density, and add it
            float3 density_l = float3(exp(-height_l / scale_height), 0.0);
            density_l.z = clamp((1.0 / cosh((height_absorption - height_l) / absorption_falloff)) * density_l.x, 0.0, 1.0);
            opt_l += density_l * step_size_l;

            // and increment where we are along the light ray.
            ray_pos_l += step_size_l;

        }
        // Now we need to calculate the attenuation
        // this is essentially how much light reaches the current sample point due to scattering
        float3 attn = exp(-(beta_mie * (opt_i.y + opt_l.y) + beta_ray * (opt_i.x + opt_l.x) + beta_absorption * (opt_i.z + opt_l.z)));

        // accumulate the scattered light (how much will be scattered towards the camera)
        total_ray += density.x * attn;
        total_mie += density.y * attn;

        // and increment the position on this ray
        ray_pos_i += step_size_i;
    }
    
    // calculate how much light can pass through the atmosphere
    float3 opacity = exp(-(beta_mie * opt_i.y + beta_ray * opt_i.x + beta_absorption * opt_i.z));
    
	// calculate and return the final color
    float3 color = (
        	phase_ray * beta_ray * total_ray // rayleigh color
       		+ phase_mie * beta_mie * total_mie // mie
            + opt_i.x * beta_ambient // and ambient
    ) * light_intensity + scene_color * opacity; // now make sure the background is rendered correctly

    return float4(color * atmosphereColor.rgb,1);
}

void AtmosphericScattering_float(
	float3 start, float3 dir, float3 scene_color,float3 light_dir, float3 light_intensity,
    float3 planet_position, float planet_radius, float atmo_radius, float3 beta_ray, float3 beta_mie, float3 beta_absorption, 
    float3 beta_ambient, float g, float height_ray, float height_mie, float height_absorption, float absorption_falloff,
    int steps_i, int steps_l, float atmosphereIntensity, float scatteringIntensity, float4 atmosphereColor, out float4 result)
{
    // Adal: for some reason calculations need to be made assuming that the sphere is at the origin
    float t0, t1;
    bool intersection = rayIntersectsSphere(start-planet_position, dir, float3(0,0,0), planet_radius, t0, t1);
    float4 scene = float4(scene_color, 1e12);
    float3 col = scene.xyz;

    if  (intersection) { //(intersection.y >= 0) {
         scene.w = max(t0, 0);//max(intersection.x, 0);
        // sample position, where the pixel is
        float3 sample_pos = start + (dir * intersection.x) - planet_position;
        // and the surface normal
        float3 surface_normal = normalize(sample_pos);
        // get wether this point is shadowed, + how much light scatters towards the camera according to the lommel-seelinger law
        float3 N = surface_normal;
        float3 V = -dir;
        float3 L = light_dir;
        float dotNV = max(1e-6, dot(N, V));
        float dotNL = max(1e-6, dot(N, L));
        float shadow = dotNL / (dotNL + dotNV);

        //// apply the shadow
        scene.xyz *= shadow;
    }
    col = AtmosphericScattering(start, dir, scene.w, scene.xyz, light_dir, light_intensity,
            planet_position, planet_radius, atmo_radius, beta_ray, beta_mie, beta_absorption,
            beta_ambient, g, height_ray, height_mie, height_absorption, absorption_falloff,
            steps_i, steps_l, atmosphereIntensity, scatteringIntensity, atmosphereColor);

    result = float4(col, 1);
    
}