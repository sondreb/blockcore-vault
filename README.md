<p align="center">
  <p align="center">
    <img src="https://avatars3.githubusercontent.com/u/53176002?s=200&v=4" height="100" alt="Blockcore" />
  </p>
  <h3 align="center">
    About Blockcore Vault
  </h3>
  <p align="center">
    Cross-Chain Multi-Chain Data Vault
  </p>
  <p align="center">
      <a href="https://github.com/sondreb/blockcore-vault/actions/workflows/build.yml"><img src="https://github.com/sondreb/blockcore-vault/actions/workflows/build.yml/badge.svg" /></a>
  </p>
  <p align="center"><em>Empowering users to keep their data secure and accessible in the modern age.</em></p>
</p>

# Blockcore Vault

Blockcore is commited to support the Confidential Storage W3C specification by the Identity Foundation.

Specification draft: https://identity.foundation/confidential-storage/

This is our work-in-progress to implement the standards for decentralized identity storage.

While we are commited to supporting the standard, our initial release of the software will use custom authentication and unencrypted storage.

The Blockcore Vault is also an [verifiable data registry](https://w3c.github.io/did-core/#dfn-verifiable-data-registry), that can host and provide DID Documents and DIDs.

## Deployment Modes

The Blockcore Vault can be deployed and configured in various modes, including:

- Permissionless and free public storage.
- Limited  to approved DIDs only (only allow known entities to manage data).
- Decentralized (multiple-nodes)
- Centralized (single-node).
- Filtered mode, used to present / host / marketplace a filtered set of VCs, e.g. "Crypto Company Registry" or "Restaurant Reviews".

## Development Plans

The initial version of the Blockcore Vault will be a basic unencrypted storage API that can be utilize by apps to store user's data.

The Vault will be able to do Vault-to-Vault data syncronization to ensure decentralized storage of data.

Future updates will add the confidential storage capabilities with encrypted storage.

## Tasks

- Make an docker-compose setup to configure TIG (Telegraf, InfluxDB and Grafana) setup for all incoming data items submitted to the vault.

- Finalize basic sync between vaults.

- Finalize basic query API.

- Support DID Resolving API.

- Support filtering node type, allowing anyone to host specific set of data and ignore the rest.

## UI: Create VCs

When a user create a VC, they can decide storage location. That can be local disk, their OneDrive/Google Drive synced local folder, 
or they can pick Vault to publish the VC too, and verification Vault if they want. If a "Verification Vault" is selected, then the
UI will show a loading indicator until the VC has been verified to have synced across from the "Target Vault" to the "Verification Vault".

## Resources

https://github.com/w3c-ccg/vc-http-api


## Attributions

[Sondre Bjellås](https://www.sondreb.com/)

[Mukesh Murugan](https://codewithmukesh.com/blog/pagination-in-aspnet-core-webapi/)

