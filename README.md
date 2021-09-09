## THIS REPO HAS BEEN REPLACED WITH: https://github.com/block-core/blockcore-vault

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

## Registration and Data Flow

1. Vault A adds Vault B by supplying the domain name URL for the vault.
1. Vault A stores the did-configuration.json retrieved from the .well-known URL.
1. Vault A performs DID Resolve to retrieve the DID Document for Vault B identity.
1. Whenever Vault A must send encrypted data to Vault B, it will utilize the "authentication" key provided in the DID Document. Consider if the rest of communication should be encrypted or not.
1. Vault A sends an request to Vault B to be accepted as a trusted node.
1. Vault B must approve the request, which does include the DID of Vault A. Vault B will perform download of did-configuration and then DID Resolve to get the DID Document of Vault A. Vault B can perform manual or automatic approval.
1. Upon approval, Vault B will inform Vault A which will update the local metadata.

Mutual trust between Vault has now been established.

1. Vault A 



## Features

### Data API

Update and retrieve data in the form of Verifiable Credentials. This will/should implement Confidential Storage API in the future.

### Sync API

This API is used to sync data across multiple Blockcore Vault nodes.

NOTES:

Sync should probably only happen over Web Socket protocol and not REST API. It would be slower and require a lot more orchestration to do REST based communication for data sync.

With Web Socket, the node can simply start pushing out messages with all the primary key it has, or ask to receive a list of primary keys and it would receive messages in batches.

Assumptions:

- Primary Key will be signature of the JWT. This must be verified if it's acceptable. The primary key will be left as base64url encoded to reduce processing.
- Consider adding more checks on sync to optimize the sync, like storing last sync date e.g. with the various Vault instances registered.

### Vault API

This API is the management API for the Blockcore Vault. Considering renaming this API to "Management" to avoid naming confusion. Maybe this API should be the "edv" or "dv", meaning "Encrypted Data Vault" or "Data Vault", which was the previous name of the "Confidential Storage" specification. Maybe an "Storage API" could be introduced too.

- CRUD operations for Blockcore Vault instances.
- When adding a new vault, the public available did-configuration.json is read and parsed and used to store the metadata.

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

- Support EDV/Confidential Storage specification, look to [bedrock-edv-storage](https://github.com/digitalbazaar/bedrock-edv-storage) for inspiration.

## Keys and Formats

Keys should be formatted as JSON Web Keys (JWK) and use the ES256K algorithm. Blockcore Vault does not support any additional formats in the initial development, support for alternatives will be adedd as required by the standard and what is implemented in the industry.

## UI: Create VCs

When a user create a VC, they can decide storage location. That can be local disk, their OneDrive/Google Drive synced local folder, 
or they can pick Vault to publish the VC too, and verification Vault if they want. If a "Verification Vault" is selected, then the
UI will show a loading indicator until the VC has been verified to have synced across from the "Target Vault" to the "Verification Vault".

## Future Improvements

For communication between Vault, the `libp2p` library could be utilize to get a lot more protocol support. `libp2p` is still under development and there is no .NET implementation available.

The library/specification does not currently rely decentralized identity specification, which is what Blockcore Vault is implemented upon.

## Resources

https://github.com/w3c-ccg/vc-http-api

https://github.com/digitalbazaar/bedrock-edv-storage

https://github.com/digitalbazaar/edv-client

https://github.com/digitalbazaar/bedrock-web-vc-store

## Attributions

[Sondre Bjellås](https://www.sondreb.com/)

[Mukesh Murugan](https://codewithmukesh.com/blog/pagination-in-aspnet-core-webapi/)

