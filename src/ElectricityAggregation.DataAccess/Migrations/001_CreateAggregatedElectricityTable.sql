CREATE TABLE IF NOT EXISTS aggregated_electricity (
                                                      region TEXT NOT NULL,
                                                      month DATE NOT NULL,
                                                      total_power_consumed NUMERIC NOT NULL,
                                                      total_power_produced NUMERIC NOT NULL,
                                                      record_count INTEGER NOT NULL,
                                                      processed_at_utc TIMESTAMP NOT NULL,

                                                      CONSTRAINT pk_aggregated_electricity PRIMARY KEY (region, month)
    );

CREATE INDEX IF NOT EXISTS idx_aggregated_electricity_month
    ON aggregated_electricity (month);

CREATE INDEX IF NOT EXISTS idx_aggregated_electricity_region
    ON aggregated_electricity (region);
