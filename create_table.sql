create type player_role as enum ('player', 'spectator');

alter type player_role owner to postgres;

create table player
(
    id       serial
        constraint player_pk
            primary key,
    username varchar     not null,
    role     player_role not null,
    token    varchar
);

alter table player
    owner to postgres;

create unique index player_username_uindex
    on player (username);

create table game
(
    id      serial
        constraint game_pk
            primary key,
    history varchar(255)
);

alter table game
    owner to postgres;

create table stats
(
    id        serial
        constraint stats_pk
            primary key,
    res       varchar not null,
    game_id   integer not null
        constraint stats_game_id_fk
            references game,
    player_id integer not null
        constraint stats_player_id_fk
            references player
);

alter table stats
    owner to postgres;

