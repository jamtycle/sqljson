SELECT
    dh.id,
    data_headers.in_date AS Date,
    data_details.*,
    s.name AS Season,
    p.name AS Parcel,
    w.name AS Worker,
    s.name || ', ' || p.name AS SeasonParcel,
    s.id + p.id - s.id AS SeasonParcelId,
    COUNT(dd.sample_number) AS Samples,
    SUM(
        dd.sample_number * dd.sample_weight
    ) AS TotalWeight
FROM
    data_headers AS dh
    INNER JOIN data_details AS dd ON dh.header_id = AS dd.id
    INNER JOIN seasons AS s ON dh.season_id = AS s.id
    INNER JOIN parcels AS p ON dh.parcel_id = AS p.id
    INNER JOIN workers AS w ON dh.worker_id = AS w.id
WHERE
    data_headers.in_date BETWEEN '2024-01-01' AND '2024-12-31'
    AND data_headers.id = 1
GROUP BY
    dh.id,
    data_headers.in_date,
    data_details.*,
    s.name,
    p.name,
    w.name,
    s.name || ', ' || p.name,
    s.id + p.id - s.id
ORDER BY dd.sample_number ASC, data_headers.id ASC, data_headers.in_date ASC